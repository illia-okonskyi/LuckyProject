module.exports = {
  root: true,
  env: { browser: true, es2020: true },
  extends: [
    'eslint:recommended',
    'plugin:react/recommended',
    'plugin:react/jsx-runtime',
    'plugin:react-hooks/recommended',
    'plugin:import/recommended',
  ],
  ignorePatterns: ['dist', '.eslintrc.cjs'],
  parserOptions: { ecmaVersion: 'latest', sourceType: 'module' },
  settings: {
    react: { version: '18.2' },
    'import/resolver': {
      node: {
        paths: ['src'],
        extensions: ['.js', '.jsx', '.ts', '.d.ts', '.tsx'],
      }
    },
  },
  plugins: ['react-refresh'],
  rules: {
    'semi': ['error', 'always'],
    'quotes': ['error', 'double', { 'avoidEscape': true }],
    'import/extensions': [
      'error',
      'never',
      {
        'svg': 'always',
        'css': 'aways'
      }],
    'react/jsx-no-target-blank': 'off',
    'react-refresh/only-export-components': [
      'warn',
      { allowConstantExport: true },
    ],
    'react/prop-types': 'off'
  },
}
