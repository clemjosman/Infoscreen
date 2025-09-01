import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NewsSlideshowPreviewService {
  
  // Configurations d'appareils prédéfinis
  private deviceConfigs = {
    tv: [
      { name: 'TV Full HD', width: 1920, height: 1080, type: 'tv' },
      { name: 'TV 4K', width: 3840, height: 2160, type: 'tv' },
      { name: 'TV HD Ready', width: 1280, height: 720, type: 'tv' },
      { name: 'TV carré', width: 1024, height: 768, type: 'tv' }
    ],
    mobile: [
      { name: 'iPhone 14 Pro', width: 393, height: 852, type: 'mobile' },
      { name: 'iPhone SE', width: 375, height: 667, type: 'mobile' },
      { name: 'Samsung Galaxy S21', width: 384, height: 854, type: 'mobile' },
      { name: 'iPad Pro', width: 1024, height: 1366, type: 'mobile' },
      { name: 'iPad Mini', width: 768, height: 1024, type: 'mobile' }
    ]
  };

  constructor() { }

  getDeviceConfigs(type: 'tv' | 'mobile') {
    return this.deviceConfigs[type];
  }

  getAllDeviceConfigs() {
    return {
      tv: this.deviceConfigs.tv,
      mobile: this.deviceConfigs.mobile
    };
  }

  openPreview(newsData: any, deviceData: any): Window | null {
    // Encoder les données
    const newsDataStr = encodeURIComponent(JSON.stringify(newsData));
    const deviceDataStr = encodeURIComponent(JSON.stringify(deviceData));

    // Construire l'URL
    const previewUrl = `/news-slideshow-preview?newsData=${newsDataStr}&deviceData=${deviceDataStr}`;
    
    // Calculer les dimensions de la fenêtre (avec marges pour l'interface)
    const windowWidth = Math.min(deviceData.width + 100, window.screen.width - 100);
    const windowHeight = Math.min(deviceData.height + 200, window.screen.height - 100);
    
    // Centrer la fenêtre
    const left = (window.screen.width - windowWidth) / 2;
    const top = (window.screen.height - windowHeight) / 2;
    
    // Options de la fenêtre
    const windowFeatures = [
      `width=${windowWidth}`,
      `height=${windowHeight}`,
      `left=${left}`,
      `top=${top}`,
      'toolbar=no',
      'location=no',
      'directories=no',
      'status=no',
      'menubar=no',
      'scrollbars=yes',
      'resizable=yes'
    ].join(',');
    
    // Ouvrir la fenêtre
    const previewWindow = window.open(previewUrl, 'NewsSlideshowPreview', windowFeatures);
    
    if (previewWindow) {
      previewWindow.focus();
    }
    
    return previewWindow;
  }

  // Méthode pour obtenir les thèmes disponibles
  getAvailableThemes() {
    return [
      { id: 'default', name: 'Défaut', primaryColor: '#0073ff' },
      { id: 'dark', name: 'Sombre', primaryColor: '#1a1a1a' },
      { id: 'actemium', name: 'Actemium', primaryColor: '#00458f' },
      { id: 'vinci', name: 'Vinci', primaryColor: '#003f7f' }
    ];
  }
}