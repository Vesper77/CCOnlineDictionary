(function ($) {
    $(function () {
        var attrs = {
            lessonId : 'data-lesson-id'
        };
        var currentLessonId = null;
        function openHomeWork(lessonId) {
            if (window.date) {
                var options = {};
                options.data = {
                    'ScheduleLessonId': lessonId,
                    'StartWeek' : window.date.startWeek
                };
                options.method = "POST";
                $.ajax(urls.getHomeWork, options).done(function (response) {
                    if (response.result) {
                        var $homeWorkText = $('#' + ids['homeWorkText']);
                        if ($homeWorkText.length > 0) {
                            if ($homeWorkText.is('textarea')) {
                                $('#' + ids['homeWorkText']).val(response.text);
                            } else {
                                $('#' + ids['homeWorkText']).text(response.text);
                            }
                            $('#' + ids['homeworkModal']).modal('toggle');
                            currentLessonId = lessonId;
                        }
                    }                        
                });
            }
        }
        function sendHomeWork() {
            if (currentLessonId != null ) {
                var options = {};
                options.data = {
                    'ScheduleLessonId': currentLessonId,
                    'StartWeek': window.date.startWeek,
                    'text' : $('#' + ids['homeWorkText']).val()
                };
                options.method = "POST";
                $.ajax(urls.setHomeWork, options);
                currentLessonId = null;
            }
        }
        $('[data-lesson-id]').click(
            function () {
                openHomeWork($(this).attr(attrs['lessonId'])
            );
            });
        if (ids.homeWorkSend) {
            $('#' + ids['homeWorkSend']).click(function () {
                sendHomeWork();
            });
        };

    });
})(jQuery);