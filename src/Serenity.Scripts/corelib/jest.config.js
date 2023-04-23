export default {
  testEnvironment: 'jsdom',
  testMatch: ['<rootDir>/test/**/*.spec.ts*'],
  moduleNameMapper: {
    '^@/(.*)$': '<rootDir>/src/$1',
    '^@serenity-is/corelib/q$': '<rootDir>/src/q',
    '^@serenity-is/corelib/slick$': '<rootDir>/src/slick',
    '^@serenity-is/sleekgrid$': '<rootDir>/node_modules/@serenity-is/sleekgrid',
    '^@optmod/(.*)$': '<rootDir>/test/testutil/$1-testmod',
    '^jquery$': '<rootDir>/test/testutil/jquery-testmod',
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
        transform: {
          react: {
            pragma: 'h',
            pragmaFrag: 'Fragment'
          }
        }
      },
      module: {
        type: "commonjs"
      }
    }]
  }
};