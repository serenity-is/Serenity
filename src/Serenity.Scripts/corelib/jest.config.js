export default {
    testEnvironment: 'jsdom',
    testMatch: ['<rootDir>/test/**/*.spec.ts'],
    moduleNameMapper: {
        '^@/(.*)$': '<rootDir>/src/$1',
        '^@serenity-is/corelib/q$': '<rootDir>/src/q',
        '^@serenity-is/corelib/slick$': '<rootDir>/src/slick'
    },
    transform: {
        "^.+\.(t|j)sx?$": [ "@swc/jest", {
          jsc: {
            parser: {
              syntax: "typescript",
              decorators: true 
            },
            keepClassNames: true,
            experimental: {
              plugins: [["jest_workaround", {}]]
            }
          },
          module: {
            type: "commonjs"
          },
        }]
      }
};