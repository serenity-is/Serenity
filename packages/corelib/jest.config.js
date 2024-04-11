export default {
  testEnvironment: './build/jsdom-global.js',
  testMatch: ['<rootDir>/test/**/*.spec.ts*', '<rootDir>/src/**/*.spec.ts*'],
  moduleNameMapper: {
    '^@/(.*)$': '<rootDir>/src/$1',
    '^@serenity-is/sleekgrid$': '<rootDir>/node_modules/@serenity-is/sleekgrid/src/index.ts'
  },
  "coveragePathIgnorePatterns": [
    "<rootDir>/node_modules/",
    "<rootDir>/src/mocks/",
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
          plugins: [["swc_mut_cjs_exports", {}]]
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
  },
  setupFiles: [
    '<rootDir>/jest.setup.js'
  ]  
};