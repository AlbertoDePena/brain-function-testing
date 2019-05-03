'use strict';

const path = require('path');
const Webpack = require('webpack');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const { AureliaPlugin } = require('aurelia-webpack-plugin');

const resolve = filePath => path.resolve(__dirname, filePath);

module.exports = (env, argv) => {
    const isDevMode = argv.mode !== 'production';

    return {
        mode: isDevMode ? 'development' : 'production',
        devtool: isDevMode ? 'eval-source-map' : false,
        entry: {
            bundle: [
                resolve('./node_modules/@babel/polyfill'),
                resolve('./node_modules/aurelia-bootstrapper')
            ]
        },
        output: {
            path: resolve('./dist'),
            publicPath: '',
            filename: isDevMode ? '[name].js' : '[name].[chunkhash].js'
        },
        resolve: {
            extensions: ['.js'],
            modules: ['src', 'node_modules'].map(x => path.resolve(x))
        },
        module: {
            rules: [
                {
                    test: /\.js$/i,
                    exclude: resolve('./node_modules'),
                    use: {
                        loader: 'babel-loader',
                        options: {
                            presets: ['@babel/preset-env'],
                            plugins: [
                                '@babel/plugin-syntax-dynamic-import',
                                '@babel/plugin-proposal-export-namespace-from',
                                ['@babel/plugin-proposal-decorators', { 'legacy': true }],
                                ['@babel/plugin-proposal-class-properties', { 'loose' : true }]
                            ]
                        }
                    }
                },
                { test: /\.html$/i, use: 'html-loader' },
                { test: /\.css$/i, loader: 'css-loader', issuer: /\.html?$/i },
                {
                    test: /\.css$/i,
                    loader: ['style-loader', 'css-loader'],
                    issuer: /\.js$/i
                },
                {
                    test: /.(ttf|eot|svg|otf|woff|woff2|png)(\?v=[0-9]\.[0-9]\.[0-9])?$/,
                    use: [
                        {
                            loader: 'file-loader',
                            options: {
                                name: '[name].[ext]',
                                outputPath: 'fonts/',
                                publicPath: 'fonts'
                            }
                        }
                    ]
                }
            ]
        },
        plugins: [
            new AureliaPlugin({ features: { svg: false } }),
            new Webpack.DefinePlugin({ DEV_MODE: JSON.stringify(isDevMode) }),
            new Webpack.ProvidePlugin({ Promise: 'bluebird' }),
            new HtmlWebpackPlugin({ template: 'src/index.html' }),
            new CopyWebpackPlugin([{ from: 'src/content/bft.png' }])
        ]
    };
};
