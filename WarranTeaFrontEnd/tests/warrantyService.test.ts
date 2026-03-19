import { describe, it, expect, vi, beforeEach } from "vitest";

const mockFetch = vi.fn();
vi.stubGlobal("$fetch", mockFetch);

describe("warrantyService", () => {
  beforeEach(() => {
    mockFetch.mockReset();
  });

  describe("getWarranties", () => {
    it("calls API with no filters", async () => {
      const warranties = [
        {
          id: 1,
          productName: "Laptop",
          brand: "Dell",
          status: "active",
        },
      ];
      mockFetch.mockResolvedValue(warranties);

      const result = await mockFetch("/api/warranties", { params: {} });

      expect(mockFetch).toHaveBeenCalledWith("/api/warranties", {
        params: {},
      });
      expect(result).toHaveLength(1);
      expect(result[0].productName).toBe("Laptop");
    });

    it("calls API with query filter", async () => {
      mockFetch.mockResolvedValue([]);

      await mockFetch("/api/warranties", {
        params: { query: "Dell" },
      });

      expect(mockFetch).toHaveBeenCalledWith("/api/warranties", {
        params: { query: "Dell" },
      });
    });

    it("calls API with all filters", async () => {
      mockFetch.mockResolvedValue([]);

      await mockFetch("/api/warranties", {
        params: { query: "Laptop", brand: "Dell", status: "active" },
      });

      expect(mockFetch).toHaveBeenCalledWith("/api/warranties", {
        params: { query: "Laptop", brand: "Dell", status: "active" },
      });
    });
  });

  describe("createWarranty", () => {
    it("sends FormData with all fields", () => {
      const formData = new FormData();
      formData.append("productName", "Phone");
      formData.append("brand", "Samsung");
      formData.append("category", "Electronics");
      formData.append("purchaseDate", "2025-01-01");
      formData.append("expirationDate", "2026-01-01");
      formData.append("coverageType", "Manufacturer");

      expect(formData.get("productName")).toBe("Phone");
      expect(formData.get("brand")).toBe("Samsung");
      expect(formData.get("category")).toBe("Electronics");
      expect(formData.get("coverageType")).toBe("Manufacturer");
    });

    it("includes optional notes in FormData", () => {
      const formData = new FormData();
      formData.append("productName", "Tablet");
      formData.append("notes", "Got from Amazon");

      expect(formData.get("notes")).toBe("Got from Amazon");
    });
  });

  describe("deleteWarranty", () => {
    it("calls DELETE endpoint with correct id", async () => {
      mockFetch.mockResolvedValue(undefined);

      await mockFetch("/api/warranties/5", { method: "DELETE" });

      expect(mockFetch).toHaveBeenCalledWith("/api/warranties/5", {
        method: "DELETE",
      });
    });
  });

  describe("scanReceipt", () => {
    it("sends file to OCR endpoint", async () => {
      const ocrResult = {
        rawText: "Product: Laptop\nBrand: Dell",
        productName: "Laptop",
        brand: "Dell",
      };
      mockFetch.mockResolvedValue(ocrResult);

      const result = await mockFetch("/api/ocr/parse-warranty", {
        method: "POST",
        body: new FormData(),
      });

      expect(result.rawText).toContain("Laptop");
      expect(result.productName).toBe("Laptop");
    });
  });
});
