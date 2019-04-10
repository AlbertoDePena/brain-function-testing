const path = require('path');
const CopyPlugin = require('copy-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = (env, argv) => {
  const isProduction = argv.mode === 'production';
  const resolve = filePath => path.resolve(__dirname, filePath);

  return {
    mode: isProduction ? 'production' : 'development',
    devtool: isProduction ? false : 'eval-source-map',
    entry: {
      bundle: [resolve('./src/main.js')]
    },
    output: {
      path: resolve('./dist'),
      publicPath: '',
      filename: isProduction ? '[name].[chunkhash].js' : '[name].js'
    },
    resolve: {
      extensions: ['.js']
    },
    module: {
      rules: [
        {
          test: /\.js$/,
          exclude: resolve('./node_modules'),
          use: {
            loader: 'babel-loader',
            options: {
              presets: ['@babel/preset-env']
            }
          }
        },
        { test: /\.css$/i, loader: ['style-loader', 'css-loader'] }
      ]
    },
    plugins: [
      new HtmlWebpackPlugin({ template: 'src/index.html' }),
      new CopyPlugin([{ from: 'src/images/bft.png' }])]
  };
};
