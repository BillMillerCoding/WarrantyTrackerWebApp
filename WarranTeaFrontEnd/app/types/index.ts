export interface User {
  id: string;
  name: string;
  email: string;
}

export interface Warranty {
  id: number;
  productName: string;
  brand: string;
  category: string;
  purchaseDate: string;
  expirationDate: string;
  status: "active" | "expiring-soon" | "expired";
  coverageType: string;
  receiptUrl?: string | null;
  warrantyDocUrl?: string | null;
  notes?: string | null;
  productId?: number | null;
}

export interface Product {
  id: string;
  name: string;
  brand: string;
  category: string;
  warrantyDuration: string;
  warrantyType: string;
  description?: string;
  imageUrl?: string;
  score?: number;
}

export interface SearchResult {
  products: Product[];
  total: number;
}

export interface WarrantyUpload {
  productName: string;
  brand: string;
  category: string;
  purchaseDate: string;
  expirationDate: string;
  coverageType: string;
  notes?: string;
  productId?: number;
  file?: File;
}

export interface OcrResult {
  rawText: string;
  productName?: string | null;
  brand?: string | null;
  category?: string | null;
  coverageType?: string | null;
  purchaseDate?: string | null;
  expirationDate?: string | null;
  notes?: string | null;
}
