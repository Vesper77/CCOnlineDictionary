(function ($) {
    // Edit mark
    $(function() {
        var isEdit = false;
        var attrs = {
            lesson: 'data-mark-lesson',
            children: 'data-mark-children',
            date: 'data-mark-date',
            fourth: 'data-mark-fourth'
        };
        var css = {
            markSet: 'mark--set',
            inputMark: 'mark__write',
            markFinalSet: 'set--mark-final'
        }
        var ids = {
            cruancyCounter: '#mark--cruancy',
            markTotal: '#mark--total'
        };
        var currentMark = null;
        var jsonresult = [];

        function saveMark() {
            jsonresult.push(
                {
                    'childrenId': currentMark.childrenId,
                    'day': currentMark.day,
                    'lessonId': currentMark.lessonId,
                    'markValue': currentMark.$input.val()
                }
            );
            //if (currentMark != null && window.urls) {
            //    isEdit = false;
            //    var markValue = currentMark.$input.val();
            //    //Russian H                
            //    if (markValue.length > 0 && (isN(markValue)) || !isNaN(parseInt(currentMark.$input.val()))) {
            //        var $selInput = currentMark.$input;
            //        var options = {};
            //        options.data = {
            //            'childrenId': currentMark.childrenId,
            //            'day': currentMark.day,
            //            'lessonId': currentMark.lessonId,
            //            'markValue': currentMark.$input.val()
            //        };
            //        if (currentMark.fourth) {
            //            options.data.fourth = currentMark.fourth;
            //        }
            //        var url = "";
            //        options.method = "POST"
            //        if (currentMark.isFinal) {
            //            var url = window.urls.setFinalMark;
            //        } else {
            //            var url = options.data.markValue % 1 === 0 ? window.urls.setMark : window.urls.setTruancy;
            //        }
                    
            //        $.ajax(url, options).done(function (response) {
            //            if (response.result && response.markValue) {
            //                if (isN(response.markValue) && !isN(currentMark.oldValue)) {                                
            //                    plusCruancy(1);
            //                } else if (!isN(response.markValue) && isN(currentMark.oldValue)) {
            //                    plusCruancy(-1);
            //                }
            //                var $tr = $selInput.parents('tr');
            //                returnMarkToView($selInput, response.markValue);
            //                recalculateMarks($tr);
            //            } else {
            //                returnMarkToView($selInput);
            //            }
            //        }).error(function (response) {
            //            returnMarkToView($selInput);
            //        });
            //    } else {
            //        returnMarkToView(currentMark.$input,currentMark.oldValue);                    
            //    }
            //}
        }
        function setMark(childrenId, date, lessonId, $input, isFinal, fourth) {
            if (typeof isFinal === 'undefiened') {
                isFinal = false;
            }
            isEdit = true;
            currentMark = {
                'childrenId': childrenId,
                'day': date,
                'lessonId': lessonId,
                '$input': $input,
                'oldValue': $input.val(),
                'isFinal': isFinal,
                'fourth': fourth
            };
        }
        function returnMarkToView($input, val) {
            if (typeof val != 'undefined') {
                $input.replaceWith(val);
            } else {
                $input.remove();
            }
            currentMark = null;
        }
        function plusCruancy(amount) {
            var value = $(ids['cruancyCounter']).text();
            value = $.trim(value);
            value = parseInt(value);
            if(!isNaN(value))
                $(ids['cruancyCounter']).text(value += amount);
        }
        function recalculateMarks($tr) {
            if ($tr) {
                var tds = $tr.children('td.' + css['markSet']);
                var summ = 0;
                var count = 0;
                tds.each(function () {
                    var value = parseInt($(this).text());
                    if (!isNaN(value)) {
                        summ += value;
                        count++;
                    }
                });
                $(ids.markTotal).text(Math.round(summ / count));
            }
        }
        function isN(value) {
            return typeof value === 'string' && $.trim(value).toUpperCase() == 'Н';
        }
        $('td.' + css['markSet']).click(function (e) {
            var $this = $(this);
            if ($this.children('input.' + css['inputMark']).length == 0) {
                if (isEdit) {
                    saveMark();
                }
                var $input = $('<input type="text" class="' + css['inputMark'] + '" />');
                $input.keypress(function (e, data) {
                    return( (e.keyCode >= 49 && e.keyCode <= 53) || e.keyCode == 1085 || e.keyCode == 1053) && e.currentTarget.value.length == 0 ;
                });
                var markValue = $this.html();

                if (isN(markValue)) {
                    $input.val($.trim(markValue));
                } else {
                    markValue = parseInt(markValue);
                    $input.val(isNaN(markValue) ? '' : markValue);
                }
                
                $this.html($input);
                setMark($this.attr(attrs['children']), $this.attr(attrs['date']), $this.attr(attrs['lesson']), $input);
            };
        });
        $(document).click(function (e) {
            var $target = $(e.target);
            if (!$target.is('.' + css['inputMark']) && $target.children('.' + css['inputMark']).length == 0 && isEdit) {
                saveMark();
            }
        });
        $(document).keypress(function (e) {
            if (e.keyCode == 13) {
                saveMark();
            }
        });
        $('td.' + css['markFinalSet']).click(function (e) {
            var $this = $(this);
            if ($this.children('input.' + css['inputMark']).length == 0) {
                if (isEdit) {
                    saveMark();
                }
                var $input = $('<input type="text" class="' + css['inputMark'] + '" />');
                $input.keypress(function (e, data) {
                    return ((e.keyCode >= 49 && e.keyCode <= 53)) && e.currentTarget.value.length == 0;
                });
                var markValue = $this.html();

                if (isN(markValue)) {
                    $input.val($.trim(markValue));
                } else {
                    markValue = parseInt(markValue);
                    $input.val(isNaN(markValue) ? '' : markValue);
                }

                $this.html($input);
                setMark($this.attr(attrs['children']), $this.attr(attrs['date']), $this.attr(attrs['lesson']), $input, true, $this.attr(attrs['fourth']));
            };
        });
        $('#saveMarks').click(function () {
            var data = {
                'marks' : jsonresult
            };
            data.method = 'POST';
            $.ajax('/Diary/setMarks',data);
        });
    });
})(jQuery);