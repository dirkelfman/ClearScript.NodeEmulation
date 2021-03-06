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

                        standalone: 'rateProvider',
                        commondir: false,
                        //debug: true,

                        builtins: ['crypto','events', 'stream', 'util', 'path', 'url', 'string_decoder', 'events', 'net', 'punycode', 'querystring', 'dgram', 'dns', 'assert', 'tls'],
                        ignoreMissing:false,
                        insertGlobals: '__filename,__dirname'
                    }
                }
            }

        }
    });


   grunt.loadNpmTasks('grunt-browserify');

    grunt.registerTask('default', ['browserify:dist']);

   //  grunt.registerTask('default', []);
};
