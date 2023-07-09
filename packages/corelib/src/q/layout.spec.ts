import { isMobileView } from "./layout";

describe('isMobileView', () => {
    it('should return false if window is undefined', () => {
        var oldWindow = window;
        window = undefined;
        try {
            expect(isMobileView()).toBe(false);
        }
        finally {
            window = oldWindow;
        }
    });

    it('should return based on window.matchMedia result for "width < 768px"', () => {
        var matchMedia = window.matchMedia;
        try {
            var q: string;
            window.matchMedia = (query: string) => {
                q = query;
                return { matches: false } as any;
            };
            expect(isMobileView()).toBe(false);
            expect(q).toBe('(max-width: 767px)');
            q = null;
            window.matchMedia = (query: string) => {
                q = query;
                return { matches: true } as any;
            };
            expect(isMobileView()).toBe(true);
            expect(q).toBe('(max-width: 767px)');
        }
        finally {
            window.matchMedia = matchMedia;
        }
    });

    it('should check window.width if window.matchMedia not available', () => {
        var matchMedia = window.matchMedia;
        try {
            delete window.matchMedia;
            window.innerWidth = 100;
            expect(isMobileView()).toBe(true);
            window.innerWidth = 1024;
            expect(isMobileView()).toBe(false);
        }
        finally {
            window.matchMedia = matchMedia;
        }
    });


});