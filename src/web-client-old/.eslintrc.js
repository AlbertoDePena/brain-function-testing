module.exports = {
    env: {
        browser: true
    },
    globals: {
        DEV_MODE: true,
        require: true,
        module: true,
        __dirname: true
    },
    extends: 'eslint:recommended',
    parser: 'babel-eslint',
    rules: {
        quotes: ['error', 'single'],
        semi: ['error', 'always']
    }
};