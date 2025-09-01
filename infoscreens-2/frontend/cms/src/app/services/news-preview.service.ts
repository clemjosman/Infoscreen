// cms/src/app/services/news-preview.service.ts
// Service pour gÃ©rer la prÃ©visualisation des news

import { Injectable } from '@angular/core';

export interface PreviewDeviceData {
  deviceType: 'mobile' | 'tv';
  width: number;
  height: number;
}

export interface PreviewNewsData {
  id?: number;
  title: string;
  content: string;
  description?: string;
  layout: 'horizontal' | 'vertical';
  publicationDate: string;
  expirationDate?: string | null;
  isVisible?: boolean;
  categories?: string[];
  box1: { content: 'text' | 'file'; size: number; };
  box2: { content: 'text' | 'file'; size: number; };
  fileSrc?: string;
  fileExtension?: string;
  fileName?: string;
}

@Injectable({
  providedIn: 'root'
})
export class NewsPreviewService {

  // Presets pour tÃ©lÃ©visions/Ã©crans
  private readonly TV_PRESETS: PreviewDeviceData[] = [
    { deviceType: 'tv', width: 1920, height: 1080 }, // Full HD
    { deviceType: 'tv', width: 2560, height: 1440 }, // QHD
    { deviceType: 'tv', width: 3840, height: 2160 }, // 4K UHD
    { deviceType: 'tv', width: 1366, height: 768 },  // HD Ready
    { deviceType: 'tv', width: 1280, height: 720 },  // HD
    { deviceType: 'tv', width: 1600, height: 900 },  // HD+
    { deviceType: 'tv', width: 2048, height: 1152 }  // QWXGA
  ];

  // Presets pour mobiles/tablettes
  private readonly MOBILE_PRESETS: PreviewDeviceData[] = [
    { deviceType: 'mobile', width: 375, height: 667 },   // iPhone SE
    { deviceType: 'mobile', width: 414, height: 896 },   // iPhone 11
    { deviceType: 'mobile', width: 390, height: 844 },   // iPhone 12/13
    { deviceType: 'mobile', width: 393, height: 851 },   // iPhone 14
    { deviceType: 'mobile', width: 360, height: 640 },   // Samsung Galaxy S8
    { deviceType: 'mobile', width: 412, height: 915 },   // Samsung Galaxy S21
    { deviceType: 'mobile', width: 768, height: 1024 },  // iPad
    { deviceType: 'mobile', width: 820, height: 1180 },  // iPad Air
    { deviceType: 'mobile', width: 1024, height: 1366 }, // iPad Pro 12.9"
    { deviceType: 'mobile', width: 540, height: 720 },   // Surface Duo
    { deviceType: 'mobile', width: 280, height: 653 }    // Galaxy Fold (fermÃ©)
  ];

  constructor() { }

  getTvPresets(): PreviewDeviceData[] {
    return [...this.TV_PRESETS];
  }

  getMobilePresets(): PreviewDeviceData[] {
    return [...this.MOBILE_PRESETS];
  }

  getResolutionName(device: PreviewDeviceData): string {
    const { width, height } = device;

    if (device.deviceType === 'tv') {
      if (width === 3840 && height === 2160) return '4K Ultra HD';
      if (width === 2560 && height === 1440) return 'QHD 1440p';
      if (width === 1920 && height === 1080) return 'Full HD 1080p';
      if (width === 1366 && height === 768) return 'HD Ready';
      if (width === 1280 && height === 720) return 'HD 720p';
      if (width === 1600 && height === 900) return 'HD+ 900p';
      if (width === 2048 && height === 1152) return 'QWXGA';
      return `${width}Ã—${height}`;
    } 
  }

  isValidDimensions(width: number, height: number): boolean {
    return width >= 320 && width <= 7680 && 
           height >= 240 && height <= 4320;
  }

  getAspectRatio(width: number, height: number): string {
    const gcd = (a: number, b: number): number => b === 0 ? a : gcd(b, a % b);
    const divisor = gcd(width, height);
    const ratioW = width / divisor;
    const ratioH = height / divisor;
    
    // Ratios courants
    if (ratioW === 16 && ratioH === 9) return '16:9';
    if (ratioW === 4 && ratioH === 3) return '4:3';
    if (ratioW === 21 && ratioH === 9) return '21:9';
    if (ratioW === 32 && ratioH === 9) return '32:9';
    if (ratioW === 3 && ratioH === 2) return '3:2';
    if (ratioW === 5 && ratioH === 4) return '5:4';
    
    return `${ratioW}:${ratioH}`;
  }

  createCustomDevice(deviceType: 'mobile' | 'tv', width: number, height: number): PreviewDeviceData {
    return {
      deviceType,
      width: Math.max(320, Math.min(width, 7680)),
      height: Math.max(240, Math.min(height, 4320))
    };
  }

  validateNewsData(newsData: PreviewNewsData): { isValid: boolean; errors: string[] } {
    const errors: string[] = [];

    if (!newsData.title || newsData.title.trim() === '') {
      errors.push('Le titre est requis');
    }

    if (!newsData.content || newsData.content.trim() === '') {
      errors.push('Le contenu est requis');
    }

    if (!newsData.layout || !['horizontal', 'vertical'].includes(newsData.layout)) {
      errors.push('Le layout doit Ãªtre horizontal ou vertical');
    }

    if (!newsData.box1 || !newsData.box2) {
      errors.push('La configuration des boxes est requise');
    } else {
      if (newsData.box1.size < 0 || newsData.box1.size > 100) {
        errors.push('La taille de box1 doit Ãªtre entre 0 et 100');
      }
      if (newsData.box2.size < 0 || newsData.box2.size > 100) {
        errors.push('La taille de box2 doit Ãªtre entre 0 et 100');
      }
      if (Math.abs((newsData.box1.size + newsData.box2.size) - 100) > 0.1) {
        errors.push('La somme des tailles des boxes doit Ãªtre Ã©gale Ã  100');
      }
      if (!['text', 'file'].includes(newsData.box1.content)) {
        errors.push('Le contenu de box1 doit Ãªtre text ou file');
      }
      if (!['text', 'file'].includes(newsData.box2.content)) {
        errors.push('Le contenu de box2 doit Ãªtre text ou file');
      }
    }

    return {
      isValid: errors.length === 0,
      errors
    };
  }

  // MÃ©thode utilitaire pour formater les donnÃ©es pour la prÃ©visualisation
  formatNewsForPreview(formData: any, currentLanguage: string = 'fr'): PreviewNewsData {
    return {
      id: formData.id || 0,
      title: this.extractTranslatedValue(formData.title, currentLanguage) || 'Titre de prÃ©visualisation',
      content: this.extractTranslatedValue(formData.contentHTML, currentLanguage) || '<p>Contenu de prÃ©visualisation</p>',
      description: formData.description || '',
      layout: formData.layout || 'horizontal',
      publicationDate: formData.publicationDate || new Date().toISOString(),
      expirationDate: formData.expirationDate || null,
      isVisible: formData.isVisible ?? true,
      categories: formData.categories || [],
      box1: formData.box1 || { content: 'file', size: 70 },
      box2: formData.box2 || { content: 'text', size: 30 },
      fileSrc: formData.attachment?.fileSrc || null,
      fileExtension: formData.attachment?.fileExtension || null,
      fileName: formData.attachment?.fileName || null
    };
  }

  private extractTranslatedValue(multilingualObj: any, language: string): string {
    if (!multilingualObj || typeof multilingualObj !== 'object') {
      return '';
    }

    // Essayer la langue demandÃ©e
    if (multilingualObj[language]) {
      return multilingualObj[language];
    }

    // Essayer d'autres langues comme fallback
    const fallbackLanguages = ['fr', 'en', 'de', 'it'];
    for (const fallbackLang of fallbackLanguages) {
      if (multilingualObj[fallbackLang]) {
        return multilingualObj[fallbackLang];
      }
    }

    // Prendre la premiÃ¨re valeur disponible
    const values = Object.values(multilingualObj);
    return values.length > 0 ? values[0] as string : '';
  }
}