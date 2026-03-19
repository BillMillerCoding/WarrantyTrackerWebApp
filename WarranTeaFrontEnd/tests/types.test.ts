import { describe, it, expect } from "vitest";
import type {
  User,
  Warranty,
  Product,
  ProductWarranty,
  WarrantyUpload,
  OcrResult,
} from "~/types";

describe("TypeScript interfaces", () => {
  describe("User", () => {
    it("creates a valid user object", () => {
      const user: User = {
        id: "abc-123",
        name: "Test User",
        email: "test@example.com",
      };
      expect(user.id).toBe("abc-123");
      expect(user.name).toBe("Test User");
      expect(user.email).toBe("test@example.com");
      expect(user.roles).toBeUndefined();
    });

    it("creates a user with admin role", () => {
      const admin: User = {
        id: "admin-1",
        name: "Admin",
        email: "admin@test.com",
        roles: ["Admin"],
      };
      expect(admin.roles).toContain("Admin");
    });
  });

  describe("Warranty", () => {
    it("creates a valid warranty object", () => {
      const warranty: Warranty = {
        id: 1,
        productName: "Laptop",
        brand: "Dell",
        category: "Electronics",
        purchaseDate: "2025-01-01",
        expirationDate: "2026-01-01",
        status: "active",
        coverageType: "Manufacturer",
      };
      expect(warranty.id).toBe(1);
      expect(warranty.status).toBe("active");
      expect(warranty.receiptUrl).toBeUndefined();
    });

    it("accepts all status values", () => {
      const statuses: Warranty["status"][] = [
        "active",
        "expiring-soon",
        "expired",
      ];
      statuses.forEach((s) => {
        expect(["active", "expiring-soon", "expired"]).toContain(s);
      });
    });
  });

  describe("ProductWarranty", () => {
    it("creates a product warranty with nested product", () => {
      const pw: ProductWarranty = {
        id: 1,
        productId: 10,
        coverageType: "Extended",
        durationMonths: 24,
        provider: "SquareTrade",
        product: {
          id: 10,
          name: "MacBook",
          brand: "Apple",
          category: "Electronics",
        },
      };
      expect(pw.product?.name).toBe("MacBook");
      expect(pw.durationMonths).toBe(24);
    });
  });

  describe("WarrantyUpload", () => {
    it("creates upload data with optional file", () => {
      const upload: WarrantyUpload = {
        productName: "Phone",
        brand: "Samsung",
        category: "Electronics",
        purchaseDate: "2025-06-01",
        expirationDate: "2026-06-01",
        coverageType: "Manufacturer",
      };
      expect(upload.file).toBeUndefined();
      expect(upload.notes).toBeUndefined();
    });
  });

  describe("OcrResult", () => {
    it("creates an OCR result with extracted fields", () => {
      const result: OcrResult = {
        rawText: "Product: Laptop\nBrand: Dell",
        productName: "Laptop",
        brand: "Dell",
      };
      expect(result.rawText).toContain("Laptop");
      expect(result.productName).toBe("Laptop");
      expect(result.purchaseDate).toBeUndefined();
    });

    it("creates an OCR result with all null fields", () => {
      const result: OcrResult = {
        rawText: "random text",
        productName: null,
        brand: null,
        category: null,
        coverageType: null,
        purchaseDate: null,
        expirationDate: null,
        notes: null,
      };
      expect(result.rawText).toBe("random text");
      expect(result.productName).toBeNull();
    });
  });
});
