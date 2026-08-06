import { Column } from "../../src/core";
import { SleekGrid } from "../../src/grid";

const getTestColumns = (): Column[] => ([
  { id: 'col1', field: 'col1', name: 'Column 1', width: 150, minWidth: 30 },
  { id: 'col2', field: 'col2', name: 'Column 2', width: 150, minWidth: 30 },
  { id: 'col3', field: 'col3', name: 'Column 3', width: 150, minWidth: 30 }
]);

const getTestData = () => ([
  { col1: 'A1', col2: 'B1', col3: 'C1' },
  { col1: 'A2', col2: 'B2', col3: 'C2' },
  { col1: 'A3', col2: 'B3', col3: 'C3' }
]);

function simulateDrag(
  element: HTMLElement | Document,
  startX: number,
  startY: number,
  endX: number,
  endY: number
): void {
  const mousedown = new MouseEvent('mousedown', { bubbles: true, clientX: startX, clientY: startY });
  const mousemove = new MouseEvent('mousemove', { bubbles: true, clientX: endX, clientY: endY });
  const mouseup = new MouseEvent('mouseup', { bubbles: true, clientX: endX, clientY: endY });

  element.dispatchEvent(mousedown);
  document.dispatchEvent(mousemove);
  document.dispatchEvent(mouseup);
}

function mockOffsetWidth(header: HTMLElement, width: number): void {
  Object.defineProperty(header, 'offsetWidth', {
    get: () => width,
    configurable: true
  });
}

describe('RTL Mode', () => {
  let container: HTMLElement;
  let grid: SleekGrid;
  let columns: Column[];
  let data: any[];

  function createGrid(options: any = {}) {
    columns = getTestColumns();
    data = getTestData();
    const defaultOptions = {
      enableColumnReorder: true,
      enableColumnResize: true,
      rtl: true
    };
    grid = new SleekGrid(container, data, columns, {
      ...defaultOptions,
      ...options
    });
    if (grid.render) grid.render();
  }

  beforeEach(() => {
    container = document.createElement('div');
    container.style.width = '800px';
    container.style.height = '400px';
    document.body.appendChild(container);
  });

  afterEach(() => {
    if (grid && grid.destroy) grid.destroy();
    if (container.parentNode) container.parentNode.removeChild(container);
  });

  describe('Column Resize in RTL Mode', () => {
    it('should increase column width when dragging the resize handle to the left', () => {
      createGrid({ rtl: true });

      const header = grid.getHeaderColumn(columns[0].id);
      mockOffsetWidth(header, 150);

      const resizeHandle = header?.querySelector('.slick-resizable-handle') as HTMLElement;
      expect(resizeHandle).toBeTruthy();

      const initialWidth = columns[0].width;

      const rect = resizeHandle.getBoundingClientRect();
      const startX = rect.left + rect.width / 2;
      const startY = rect.top + rect.height / 2;

      // Drag left by 50px
      simulateDrag(resizeHandle, startX, startY, startX - 50, startY);

      expect(columns[0].width).toBeGreaterThan(initialWidth);
    });

    it('should decrease column width when dragging the resize handle to the right', () => {
      createGrid({ rtl: true });

      const header = grid.getHeaderColumn(columns[0].id);
      mockOffsetWidth(header, 150);

      expect(header).toBeTruthy();
      const resizeHandle = header?.querySelector('.slick-resizable-handle') as HTMLElement;
      expect(resizeHandle).toBeTruthy();

      const initialWidth = columns[0].width;

      const rect = resizeHandle.getBoundingClientRect();
      const startX = rect.left + rect.width / 2;
      const startY = rect.top + rect.height / 2;

      // Drag right by 50px
      simulateDrag(resizeHandle, startX, startY, startX + 50, startY);

      expect(columns[0].width).toBeLessThan(initialWidth);
    });

    it('should respect minimum column width when decreasing (drag right)', () => {
      createGrid({ rtl: true });

      const header = grid.getHeaderColumn(columns[0].id);
      mockOffsetWidth(header, 150);
      
      const resizeHandle = header?.querySelector('.slick-resizable-handle') as HTMLElement;
      expect(resizeHandle).toBeTruthy();

      const rect = resizeHandle.getBoundingClientRect();
      const startX = rect.left + rect.width / 2;
      const startY = rect.top + rect.height / 2;

      // Drag right by 140px (should stop at minWidth)
      simulateDrag(resizeHandle, startX, startY, startX + 140, startY);

      expect(columns[0].width).toBeGreaterThanOrEqual(25);
      expect(columns[0].width).toBeLessThanOrEqual(45); // Allowing some tolerance for the drag simulation
    });
  });

  it('should allow each column expand after column is at minWidth in RTL (raw mouse events)', () => {
    createGrid({ rtl: true });

    const fireMouseEvent = (type: string, target: EventTarget, clientX: number, clientY: number) => {
      const event = new MouseEvent(type, { bubbles: true, cancelable: true, clientX, clientY });
      target.dispatchEvent(event);
    };

    // Mock offsetWidth for each header according to its current model width
    const mockHeadersToCurrentWidths = () => {
      columns.forEach(col => {
        const hdr = grid.getHeaderColumn(col.id);
        if (hdr) mockOffsetWidth(hdr, col.width);
      });
    };

    // Test each column (0, 1, 2)
    for (let i = 0; i < columns.length; i++) {
      
      // Reset all column widths to 150
      columns.forEach(c => c.width = 150);
      mockHeadersToCurrentWidths();

      const header = grid.getHeaderColumn(columns[i].id);
      const resizeHandle = header?.querySelector('.slick-resizable-handle') as HTMLElement;
      expect(resizeHandle).toBeTruthy();

      let rect = resizeHandle.getBoundingClientRect();
      let startX = rect.left + rect.width / 2;
      let startY = rect.top + rect.height / 2;

      // Shrink all columns 0..i to minWidth. Total shrink capacity = (i+1) * (150 - 30)
      const totalShrink = (i + 1) * 120;
      fireMouseEvent('mousedown', resizeHandle, startX, startY);
      fireMouseEvent('mousemove', document, startX + totalShrink, startY);
      fireMouseEvent('mouseup', document, startX + totalShrink, startY);

      // All columns 0..i should be near minWidth (30)
      for (let k = 0; k <= i; k++) {
        expect(columns[k].width).toBeGreaterThanOrEqual(25);
        expect(columns[k].width).toBeLessThanOrEqual(45);
      }
      // Columns > i remain unchanged (150)
      for (let k = i + 1; k < columns.length; k++) {
        expect(columns[k].width).toBeGreaterThanOrEqual(145);
        expect(columns[k].width).toBeLessThanOrEqual(155);
      }

      // Update the offsetWidth mock to reflect the current widths
      mockHeadersToCurrentWidths();

      // Re‑fetch handle position (layout may have changed)
      rect = resizeHandle.getBoundingClientRect();
      startX = rect.left + rect.width / 2;
      startY = rect.top + rect.height / 2;

      // Second drag: expand by dragging left by 200px
      fireMouseEvent('mousedown', resizeHandle, startX, startY);
      fireMouseEvent('mousemove', document, startX - 200, startY);
      fireMouseEvent('mouseup', document, startX - 200, startY);

      // The dragged column (i) should expand to near 230 (220-250)
      expect(columns[i].width).toBeGreaterThanOrEqual(220);
      expect(columns[i].width).toBeLessThanOrEqual(250);
      // Columns before i stay at minWidth 30 (25-45)
      for (let k = 0; k < i; k++) {
        expect(columns[k].width).toBeGreaterThanOrEqual(25);
        expect(columns[k].width).toBeLessThanOrEqual(45);
      }
      // Columns after i stay at 150
      for (let k = i + 1; k < columns.length; k++) {
        expect(columns[k].width).toBeGreaterThanOrEqual(145);
        expect(columns[k].width).toBeLessThanOrEqual(155);
      }
    }
  });

  describe('LTR Regression', () => {
    it('should behave correctly in LTR mode (drag right increases, drag left decreases)', () => {
      createGrid({ rtl: false });

      const header = grid.getHeaderColumn(columns[0].id);
      mockOffsetWidth(header, 150);

      const resizeHandle = header?.querySelector('.slick-resizable-handle') as HTMLElement;
      expect(resizeHandle).toBeTruthy();

      const initialWidth = columns[0].width;

      const rect = resizeHandle.getBoundingClientRect();
      const startX = rect.left + rect.width / 2;
      const startY = rect.top + rect.height / 2;

      // Drag right should increase
      simulateDrag(resizeHandle, startX, startY, startX + 50, startY);
      expect(columns[0].width).toBeGreaterThan(initialWidth);

      // Reset width and drag left should decrease
      columns[0].width = initialWidth;
      simulateDrag(resizeHandle, startX, startY, startX - 50, startY);
      expect(columns[0].width).toBeLessThan(initialWidth);
    });
  });
});
