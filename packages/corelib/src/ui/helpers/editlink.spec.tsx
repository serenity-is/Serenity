import { EditLink } from './editlink';

describe('EditLink', () => {
    it('should render with default properties', () => {
        const link = EditLink({ itemId: 123 });
        expect(link).toStrictEqual(<a class="s-EditLink" data-item-id="123" href="#/123" />);
    });

    it('should render with itemType', () => {
        const link = EditLink({ itemId: 123, itemType: 'Northwind.Customer' });
        expect(link).toStrictEqual(<a class="s-EditLink s-Northwind-Customer-Link" data-item-type="Northwind.Customer" data-item-id="123" href="#Northwind-Customer/123" />);
    });

    it('should render with cssClass', () => {
        const link = EditLink({ itemId: 123, cssClass: 'custom-class' });
        expect(link).toStrictEqual(<a class="s-EditLink custom-class" data-item-id="123" href="#/123" />);
    });

    it('should render with both itemType and cssClass', () => {
        const link = EditLink({ itemId: 123, itemType: 'Northwind.Customer', cssClass: 'custom-class' });
        expect(link).toStrictEqual(<a class="s-EditLink s-Northwind-Customer-Link custom-class" data-item-type="Northwind.Customer" data-item-id="123" href="#Northwind-Customer/123" />);
    });

    it('should render with tabIndex', () => {
        const link = EditLink({ itemId: 123, tabIndex: 5 });
        expect(link).toStrictEqual(<a class="s-EditLink" data-item-id="123" href="#/123" tabindex={5} />);
    });

    it('should render with children', () => {
        const link = EditLink({ itemId: 123, children: 'Edit' });
        expect(link).toStrictEqual(<a class="s-EditLink" data-item-id="123" href="#/123">Edit</a>);
    });

    it('should render with complex children', () => {
        const link = EditLink({ itemId: 123, children: <span>Edit Customer</span> });
        expect(link).toStrictEqual(<a class="s-EditLink" data-item-id="123" href="#/123"><span>Edit Customer</span></a>);
    });

    it('should render with all properties', () => {
        const link = EditLink({
            itemId: 456,
            itemType: 'Test.Module',
            cssClass: 'btn btn-primary',
            tabIndex: 10,
            children: 'Full Edit'
        });
        expect(link).toStrictEqual(<a class="s-EditLink s-Test-Module-Link btn btn-primary" data-item-type="Test.Module" data-item-id="456" href="#Test-Module/456" tabindex={10}>Full Edit</a>);
    });

    it('should handle null itemId', () => {
        const link = EditLink({ itemId: null });
        expect(link).toStrictEqual(<a class="s-EditLink" href='#/' />);
    });

    it('should handle undefined itemId', () => {
        const link = EditLink({ itemId: undefined });
        expect(link).toStrictEqual(<a class="s-EditLink" href="#/" />);
    });

    it('should handle zero itemId', () => {
        const link = EditLink({ itemId: 0 });
        expect(link).toStrictEqual(<a class="s-EditLink" data-item-id="0" href="#/0" />);
    });

    it('should handle string itemId', () => {
        const link = EditLink({ itemId: 'abc-123' });
        expect(link).toStrictEqual(<a class="s-EditLink" data-item-id="abc-123" href="#/abc-123" />);
    });

    it('should handle itemType with multiple dots', () => {
        const link = EditLink({ itemId: 123, itemType: 'Very.Long.Namespace.Type' });
        expect(link).toStrictEqual(<a class="s-EditLink s-Very-Long-Namespace-Type-Link" data-item-type="Very.Long.Namespace.Type" data-item-id="123" href="#Very-Long-Namespace-Type/123" />);
    });

    it('should handle empty itemType', () => {
        const link = EditLink({ itemId: 123, itemType: '' });
        expect(link).toStrictEqual(<a class="s-EditLink" data-item-type="" data-item-id="123" href="#/123" />);
    }); it('should handle itemType without dots', () => {
        const link = EditLink({ itemId: 123, itemType: 'SimpleType' });
        expect(link).toStrictEqual(<a class="s-EditLink s-SimpleType-Link" data-item-type="SimpleType" data-item-id="123" href="#SimpleType/123" />);
    });

    it('should handle empty cssClass', () => {
        const link = EditLink({ itemId: 123, cssClass: '' });
        expect(link).toStrictEqual(<a class="s-EditLink" data-item-id="123" href="#/123" />);
    });

    it('should handle multiple css classes', () => {
        const link = EditLink({ itemId: 123, cssClass: 'class1 class2 class3' });
        expect(link).toStrictEqual(<a class="s-EditLink class1 class2 class3" data-item-id="123" href="#/123" />);
    });

    it('should handle tabindex zero', () => {
        const link = EditLink({ itemId: 123, tabIndex: 0 });
        expect(link).toStrictEqual(<a class="s-EditLink" data-item-id="123" href="#/123" tabindex={0} />);
    });

    it('should handle negative tabindex', () => {
        const link = EditLink({ itemId: 123, tabIndex: -1 });
        expect(link).toStrictEqual(<a class="s-EditLink" data-item-id="123" href="#/123" tabindex={-1} />);
    });
});