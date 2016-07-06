(function ($) {
    $(function () {
        var attrs = {
            lessonId : 'data-lesson-id'
        };
        var currentLessonId = null;
        var homeWorkId = null;
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
                        if (response.homeWorkId) {
                            homeWorkId = response.homeWorkId;
                            if (ids.homeWorkReady)
                                $('#' + ids['homeWorkReady']).show();
                        } else if(ids.homeWorkReady) {
                            $('#' + ids['homeWorkReady']).hide();
                        }
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
        function readyHomeWork() {
            if(homeWorkId != null && urls.readyHomeWork){
                var options = {};
                options.data = {
                    homeWorkId: homeWorkId
                };
                options.Methood = "POST";
                $.ajax(urls.readyHomeWork, options);
            }            
        }
        $('[data-lesson-id]').click(
            function () {
                openHomeWork($(this).attr(attrs['lessonId'])
            );
        });
        if (ids.homeWorkSend) {
            $('#' + ids['homeWorkSend']).click(sendHomeWork);
        }
        if (ids.homeWorkReady) {
            $('#' + ids['homeWorkReady']).click(readyHomeWork); 
        }
    });
})(jQuery);