/// <binding BeforeBuild='sass:dev' AfterBuild='concat:dev' />
module.exports = function (grunt) {
	require('load-grunt-tasks')(grunt);

	grunt.initConfig({
		pkg: grunt.file.readJSON("package.json"),
		
		sass: {
			dev: {
				files: {
					"Content/dist/site.css": "Content/src/scss/site.scss"
				}
			},
			dist: {
				files: {
					"Content/dist/site.css": "Content/src/scss/site.scss"
				}
			}
		}
	});
};
