(function() {
    if (typeof $ !== "undefined" && $.fn && $.fn.ajaxStart && $.fn.ajaxStop && typeof NProgress !== "undefined") {
        var npt;

        $(document).ajaxStart(function () {
            clearTimeout(npt);
            npt = setTimeout(NProgress.start, 200);
        });

        $(document).ajaxStop(function () {
            clearTimeout(npt);
            NProgress.done();
        });
    }

    function setCookie(name, value) {
        name = encodeURIComponent(name);

        if (value == null) {
            document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/";
            return;
        }

        var date = new Date();
        date.setFullYear(date.getFullYear() + 1);
        document.cookie = name + "=" + encodeURIComponent(value) +
            ";expires=" + date.toGMTString() + ";path=/";
    }

    function setupDataGridDefaults() {
        Serenity.DataGrid.defaultRowHeight = 36;
        Serenity.DataGrid.defaultHeaderHeight = 34;
        Serenity.DataGrid.defaultColumnWidthDelta = 10;
        Serenity.DataGrid.defaultColumnWidthScale = 14 / 13;
    }

    if (typeof Serenity !== "undefined") {
        setupDataGridDefaults();
    }
        
    document.addEventListener('DOMContentLoaded', function () {

        var languageMenus = document.querySelectorAll('.s-language-selection-menu')
        if (languageMenus.length && typeof Q != "undefined" && Q.getLookupAsync) {

            function languageClick(e) {
                var a = e.target.closest('[data-language]');
                if (!a)
                    return;

                setCookie("LanguagePreference", a.getAttribute('data-language'));
                window.location.reload(true);
            }

            languageMenus.forEach(function (menu) {
                menu.addEventListener('click', languageClick);
            });

            Q.getLookupAsync("Administration.Language").then(function (lookup) {
                for (var l of lookup.items) {
                    languageMenus.forEach(function (menu) {
                        var li = document.createElement('li');
                        var a = li.appendChild(document.createElement('a'));
                        a.classList.add('dropdown-item');
                        a.setAttribute('data-language', l.LanguageId);
                        a.setAttribute('href', 'javascript:;');
                        a.textContent = l.LanguageName;
                        menu.appendChild(li);
                    });
                }
            });
        }

        var toggler = document.querySelector('#s-sidebar-toggler');
        toggler && toggler.addEventListener('click', function (e) {

            document.body.classList.toggle('s-sidebar-expanded');

            document.addEventListener('mouseup', function (e) {
                if (!e.target ||
                    (!e.target.closest('.s-sidebar') &&
                        !e.target.closest('#s-sidebar-toggler')))
                    document.body.classList.remove('s-sidebar-expanded');
            });
        });

        var getParents = function (elem) {
            var parents = [];
            for (; elem && elem !== document; elem = elem.parentNode) {
                parents.push(elem);
            }
            return parents;
        };

        var searchInput = document.querySelector('.s-sidebar-search-input');

        function searchInputChange() {
            var searchText = (searchInput.value || '').trim();

            var sidebarItems = document.querySelectorAll('.s-sidebar-item');
            sidebarItems.forEach(function (li) {
                li.classList.remove('is-match');
                li.classList.remove('non-match');
            });

            var parts = searchText.split(' ').map(function (x) {
                return x.trim();
            }).filter(function (x) {
                return x.length;
            });

            if (!parts.length) 
                return;

            var normalize;

            if (typeof Serenity !== "undefined" &&
                typeof Serenity.stripDiacritics === "function") {
                normalize = function (text) {
                    return Serenity.stripDiacritics(text.trim()).toUpperCase();
                }
            }
            else {
                normalize = function (text) {
                    return text.trim().toUpperCase();
                }
            }

            for (var i = 0; i < parts.length; i++) {
                parts[i] = normalize(parts[i]);
            }

            function getTitle(el) {
                if (el.classList.contains('s-sidebar-item')) {
                    el = el.querySelector(':scope>.s-sidebar-link .s-sidebar-link-text,:scope>.s-sidebar-section-title');
                    if (el)
                        return normalize(el.textContent);
                }
                else if (el.classList.contains('s-sidebar-group')) {
                    el = el.querySelector(':scope>.s-sidebar-group-title');
                    if (el)
                        return normalize(el.textContent);
                }

                return '';
            }

            var matches = [];

            sidebarItems.forEach(function (li) {
                var titles = [];
                getParents(li).forEach(function (el) {
                    var txt = getTitle(el);
                    if (txt.length)
                        titles.push(txt);
                });

                var match = true;
                for (var p = 0; p < parts.length; p++) {
                    var pt = parts[p];
                    if (!titles.some(function (t) {
                        return t.indexOf(pt) >= 0;
                    })) {
                        li.classList.add('non-match');
                        match = false;
                        break;
                    }
                }

                if (match)
                    matches.push(li);
            });

            matches.forEach(function (li) {
                li.classList.add('is-match');
                getParents(li.parentNode).forEach(function (p) {
                    if (p.classList.contains('s-sidebar-item')) {
                        p.classList.remove('non-match');
                        p.classList.add('is-match');
                    }
                });
            });
        }

        if (searchInput) {
            searchInput.addEventListener('keyup', searchInputChange);
            searchInput.addEventListener('change', searchInputChange);
        }
    });
})();