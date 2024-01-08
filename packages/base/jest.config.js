export default {
  testEnvironment: './build/jsdom-global.js',
  testMatch: ['<rootDir>/test/**/*.spec.ts*', '<rootDir>/src/**/*.spec.ts*'],
  moduleNameMapper: {
  },
  extensionsToTreatAsEsm: ['.ts', '.tsx'],
  transformIgnorePatterns: [],
  transform: {
    "^.+\.(t|j)sx?$": ["@swc/jest", {
      jsc: {
        parser: {
          syntax: "typescript",
          decorators: true,
          tsx: true
        },
        keepClassNames: true,
        experimental: {
          plugins: [["jest_workaround", {}]]
        },
      },
      module: {
        type: "commonjs"
      }
    }]
  }
};