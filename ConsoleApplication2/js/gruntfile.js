// SDK Gruntfile


module.exports = function(grunt) {

    grunt.initConfig({
        browserify: {
            dist: {
                files: {
                    'rateProvider.built.js': ['rateProvider.js']
                },

                options: {
            
                    
    
                    // pipeline: ['dedupe','debug'],
                    browserifyOptions: {

                        standalone:'food',
                        commondir: false,
                        debug: true,

                        builtins: ['events', 'stream', 'util', 'path', 'url', 'string_decoder', 'events', 'net', 'punycode', 'querystring', 'dgram', 'dns', 'assert', 'tls'],

                        insertGlobals: '__filename,__dirname'
                    }
                }
            }

        }
    });


    grunt.loadNpmTasks('grunt-browserify');

    grunt.registerTask('default', ['browserify:dist']);

     //grunt.registerTask('default', []);
};
