export default {
  testEnvironment: 'jsdom',
  testMatch: ['<rootDir>/test/**/*.spec.ts*', '<rootDir>/src/**/*.spec.ts*'],
  moduleNameMapper: {
    '^@/(.*)$': '<rootDir>/src/$1',
    '^@serenity-is/corelib/(.*)$': '<rootDir>/src/$1',
    '^@serenity-is/sleekgrid$': '<rootDir>/node_modules/@serenity-is/sleekgrid',
    '^@optionaldeps/(.*)$': '<rootDir>/test/testutil/$1-testmodule',
    '^jquery$': '<rootDir>/../../src/Serenity.Assets/wwwroot/jquery/jquery.min.js',
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
            runtime: 'automatic',
            importSource: 'jsx-dom'
          }
        }
      },
      module: {
        type: "commonjs"
      }
    }]
  }
};