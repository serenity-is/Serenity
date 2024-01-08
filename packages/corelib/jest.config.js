export default {
  testEnvironment: './build/jsdom-global.js',
  testMatch: ['<rootDir>/test/**/*.spec.ts*', '<rootDir>/src/**/*.spec.ts*'],
  moduleNameMapper: {
    '^@/(.*)$': '<rootDir>/src/$1',
    "^@mockdeps": '<rootDir>/src/globals/mockdeps',
    '^@serenity-is/base$': '<rootDir>/node_modules/@serenity-is/base/src/index.ts',
    '^@serenity-is/base-ui$': '<rootDir>/node_modules/@serenity-is/base-ui/src/index.ts',
    '^@serenity-is/sleekgrid$': '<rootDir>/node_modules/@serenity-is/sleekgrid/src/index.ts'
  },
  "coveragePathIgnorePatterns": [
    "<rootDir>/node_modules/",
    "/src/Serenity.Assets/"
  ],
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