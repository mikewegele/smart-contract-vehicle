import { defineConfig } from 'orval';

export default defineConfig({
    myApi: {
        input: 'http://localhost:5147/swagger/v1/swagger.json',
        output: {
            target: './src/api/index.ts',
            client: 'axios',
            override: {
                mutator: {
                    path: './src/api/axios-config.ts',
                    name: 'api',
                },
            },
        },
        hooks: true,
    },
});