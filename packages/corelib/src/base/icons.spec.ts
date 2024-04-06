import { AnyIconClass, IconClassName, KnownIconClass, bgColor, faIcon, fabIcon, iconClassName, textColor } from './icons';

describe('bgColor', () => {
    it('should return the correct background color class', () => {
        expect(bgColor('primary')).toBe('bg-primary');
        expect(bgColor('success')).toBe('bg-success');
        expect(bgColor('warning')).toBe('bg-warning');
    });
});

describe('textColor', () => {
    it('should return the correct text color class', () => {
        expect(textColor('primary')).toBe('text-primary');
        expect(textColor('success')).toBe('text-success');
        expect(textColor('warning')).toBe('text-warning');
    });
});

describe('faIcon', () => {
    it('should return the correct font awesome icon class', () => {
        expect(faIcon('address-book')).toBe('fa fa-address-book');
        expect(faIcon('coffee', 'primary')).toBe('fa fa-coffee text-primary');
    });
});

describe('fabIcon', () => {
    it('should return the correct font awesome brand icon class', () => {
        expect(fabIcon('github')).toBe('fab fa-github');
        expect(fabIcon('twitter', 'info')).toBe('fab fa-twitter text-info');
    });
});

describe('iconClassName', () => {
    it('should return the correct icon class name', () => {
        const knownIcon: KnownIconClass = 'fa fa-address-book';
        const anyIcon: AnyIconClass = 'fa fa-coffee';
        const multipleIcons: IconClassName = ['fa fa-coffee', 'fab fa-github'];

        expect(iconClassName(knownIcon)).toBe('fa fa-address-book');
        expect(iconClassName(anyIcon)).toBe('fa fa-coffee');
        expect(iconClassName(multipleIcons)).toBe('fa fa-coffee fab fa-github');
    });
});