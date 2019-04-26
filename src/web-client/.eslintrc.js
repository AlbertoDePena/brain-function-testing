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
  extends: "eslint:recommended",
  parserOptions: {
    ecmaFeatures: { legacyDecorators: true }
  },
  parser: "babel-eslint",
  rules: {
    quotes: ["error", "single"],
    semi: ["error", "always"]
  }
};
