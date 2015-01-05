// SDK Gruntfile


module.exports = function(grunt) {

    grunt.initConfig({
        browserify: {
            dist: {
                files: {
                    'actionFilters.built.js': ['actionFilters.js']
                },

                options: {
            
                    
    
                    browserifyOptions: {

                        standalone: 'actionFilters',
                        commondir: false,
                     

                        builtins: ['events', 'stream', 'util', 'path', 'url', 'string_decoder', 'events', 'net', 'punycode', 'querystring', 'dgram', 'dns', 'assert', 'tls'],

                        insertGlobals: '__filename,__dirname'
                    }
                }
            }

        }
    });


   grunt.loadNpmTasks('grunt-browserify');

    grunt.registerTask('default', ['browserify:dist']);

};
