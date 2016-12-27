// Include Gulp
var gulp = require('gulp');

// Include $
var $ = require("gulp-load-plugins")({
    pattern: ['gulp-*', 'gulp.*', 'main-bower-files'],
    replaceString: /\bgulp[\-.]/
});

// Define default destination folder
var dest = 'wwwroot/';
var destLib = 'vendor';

const bFilter = $.filter('**/*.{js,css,eot,svg,ttf,woff,woff2,less}');



gulp.task('cleanPrep', function () {
    return gulp.src([dest + 'libs'], { read: false }).pipe($.clean());
});

gulp.task('prep', ['cleanPrep'], function () {
    gulp.src($.mainBowerFiles(), { base: 'bower_components' })
		.pipe(bFilter)
		.pipe(gulp.dest(dest + destLib));
});

gulp.task("less", function () {
    return gulp.src('wwwroot/less/index.less')
        .pipe($.less())
        .pipe(gulp.dest(dest + '/css'));
});