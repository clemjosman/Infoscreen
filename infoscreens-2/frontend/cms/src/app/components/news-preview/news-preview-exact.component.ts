// cms/src/app/components/news-preview/news-preview-exact.component.ts
// ⚡ REPRODUCTION 100% EXACTE du slideshow NewsInternalComponent

import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Subscription } from 'rxjs';
import { FuseConfigService } from '@vesact/web-ui-template';

// ==========================================
// INTERFACES EXACTES du slideshow  
// ==========================================

interface PreviewDeviceData {
  deviceType: 'tv';
  width: number;
  height: number;
}

// STRUCTURE MULTILINGUE exacte du CMS
interface PreviewNewsData {
  id?: number;
  title: { [key: string]: string } | string;
  content: { [key: string]: string } | string;
  description?: string;
  layout: 'horizontal' | 'vertical';
  publicationDate: string;
  expirationDate?: string | null;
  isVisible?: boolean;
  categories?: string[];
  box1: { content: 'text' | 'file'; size: number; };
  box2: { content: 'text' | 'file'; size: number; };
  attachment?: {
    fileSrc?: string;
    fileExtension?: string;
    fileName?: string;
  };
  // ⚡ Propriétés ajoutées dynamiquement durant la transformation
  text?: { content: string; size: number; };
  file?: { content: string; size: number; };
  fileFirst?: boolean;
}

// INTERFACE EXACTE du slideshow NewsInternalComponent
interface News_Display {
  id: number;
  title: string;
  content: string;
  description: string;
  layout: 'horizontal' | 'vertical';
  publicationDate: string;
  isVisible: boolean;
  categories: string[];
  text: { content: string; size: number; };
  file: { content: string; size: number; };
  fileFirst: boolean;
  fileSrc?: string;
  fileExtension?: string;
  fileName?: string;
}

@Component({
  selector: 'app-news-preview-exact',
  templateUrl: './news-preview-exact.component.html',
  styleUrls: ['./news-preview-exact.component.scss']
})
export class NewsPreviewExactComponent implements OnInit, OnDestroy {
  
  // ==========================================
  // PROPRIÉTÉS EXACTES du slideshow NewsInternalComponent
  // ==========================================
  
  readonly DEFAULT_IMAGE_NEWS = 'assets/images/News_Default_Image.jpg';
  readonly slide: string = 'NewsInternal';
  newsFileLink: string = this.DEFAULT_IMAGE_NEWS;
  showFooter: boolean = true;
  news: News_Display | undefined;

  // Propriétés de prévisualisation
  deviceData: PreviewDeviceData;
  newsData: PreviewNewsData;
  currentLanguage: string = 'fr';
  
  // États
  isLoading: boolean = true;
  error: string | null = null;
  
  // Date/heure pour footer
  currentDate: string = '';
  currentTime: string = '';
  
  private subscriptions: Subscription = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private sanitizer: DomSanitizer,
    private _fuseConfigService: FuseConfigService,
    
 
  ) {
    // Configure the layout
        this._fuseConfigService.config = {
            layout: {
                navbar   : {
                    hidden: true
                },
                toolbar  : {
                    hidden: true
                },
                footer   : {
                    hidden: true
                },
                sidepanel: {
                    hidden: true
                }
            }
        };
  }

  ngOnInit(): void {
    this.initializeDateTime();
    this.loadPreviewData();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private initializeDateTime(): void {
    const now = new Date();
    this.currentDate = this.formatDateSlideshow(now);
    this.currentTime = this.formatTimeSlideshow(now);
  }

  private formatDateSlideshow(date: Date): string {
    // ⚡ FORMAT ALLEMAND/SUISSE comme dans le slideshow
    const days = ['Sonntag', 'Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag'];
    const months = ['Januar', 'Februar', 'März', 'April', 'Mai', 'Juni', 
                   'Juli', 'August', 'September', 'Oktober', 'November', 'Dezember'];
    
    const dayName = days[date.getDay()];
    const day = date.getDate();
    const monthName = months[date.getMonth()];
    const year = date.getFullYear();
    
    return `${dayName}, ${day}. ${monthName} ${year}`;
  }

  private formatTimeSlideshow(date: Date): string {
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
  }

  getFullDateTime(): string {
    return `${this.currentDate}, ${this.currentTime}`;
  }

  private loadPreviewData(): void {
    try {
      // Récupérer les données depuis les query params
      const newsParam = this.route.snapshot.queryParams['news'];
      const deviceParam = this.route.snapshot.queryParams['device'];
      
      if (!newsParam || !deviceParam) {
        this.error = 'Données manquantes pour la prévisualisation';
        this.isLoading = false;
        return;
      }

      this.newsData = JSON.parse(decodeURIComponent(newsParam));
      this.deviceData = JSON.parse(decodeURIComponent(deviceParam));

      console.log('📋 DONNÉES REÇUES:', {
        newsData: this.newsData,
        deviceData: this.deviceData
      });

      this.transformNewsForSlideshow();

    } catch (error) {
      console.error('❌ Erreur lors du chargement des données:', error);
      this.error = 'Erreur lors du chargement des données';
    } finally {
      this.isLoading = false;
    }
  }

  private transformNewsForSlideshow(): void {
    const news: PreviewNewsData & {
      text: { content: string; size: number; };
      file: { content: string; size: number; };
      fileFirst: boolean;
    } = { ...this.newsData } as any;


    const title = this.extractTextFromMultilingual(news.title);
    const content = this.extractTextFromMultilingual(news.content);

    console.log('🔄 TRANSFORMATION:', {
      'titre original': news.title,
      'titre extrait': title,
      'contenu original': news.content,
      'contenu extrait': content.substring(0, 100) + '...',
      'box1': news.box1,
      'box2': news.box2
    });

    // ⚡ LOGIQUE EXACTE DU SLIDESHOW (corrigée)
    switch (news.box1.content) {
      case 'text':
        news.text = news.box1;
        news.file = news.box2;
        news.fileFirst = false;
        console.log('📝 CASE TEXT: box1=text, box2=file, fileFirst=false');
        break;
      case 'file':
      default:
        news.text = news.box2;
        news.file = news.box1;
        news.fileFirst = true;
        console.log('🖼️ CASE FILE: box1=file, box2=text, fileFirst=true');
        break;
    }

    console.log('⚡ APRÈS SWITCH:', {
      fileFirst: news.fileFirst,
      'file.size': news.file.size,
      'text.size': news.text.size,
      'file.content': news.file.content,
      'text.content': news.text.content
    });

    // CONVERSION FINALE vers News_Display
    this.news = {
      id: news.id || 0,
      title: title,
      content: content,
      description: news.description || '',
      layout: news.layout,
      publicationDate: this.formatPublicationDate(news.publicationDate), // ⚡ FORMATAGE CORRECT
      isVisible: news.isVisible ?? true,
      categories: news.categories || [],
      text: news.text,
      file: news.file,
      fileFirst: news.fileFirst,
      fileSrc: news.attachment?.fileSrc,
      fileExtension: news.attachment?.fileExtension,
      fileName: news.attachment?.fileName
    };

    console.log('✅ NEWS FINALE TRANSFORMÉE:', this.news);

    // Définir le lien du fichier
    if (this.hasAttachment()) {
      this.newsFileLink = this.news.fileSrc!;
      console.log('🖼️ Lien fichier défini:', this.newsFileLink);
    } else {
      this.newsFileLink = this.DEFAULT_IMAGE_NEWS;
      console.log('🖼️ Pas de fichier, utilisation image par défaut');
    }
  }

  /**
   * Formater la date de publication comme dans le slideshow original
   */
  private formatPublicationDate(dateString: string): string {
    if (!dateString) return '';
    
    try {
      const date = new Date(dateString);
      
      // Format comme dans le slideshow original (format local)
      const day = date.getDate().toString().padStart(2, '0');
      const month = (date.getMonth() + 1).toString().padStart(2, '0');
      const year = date.getFullYear();
      
      return `${day}.${month}.${year}`;
    } catch (error) {
      console.error('❌ Erreur format date publication:', error);
      return dateString; // Fallback sur la date originale
    }
  }

  /**
   * MÉTHODE CRUCIALE : Extraire le texte depuis un objet multilingue du CMS
   */
  private extractTextFromMultilingual(textData: { [key: string]: string } | string): string {
    console.log('🔍 Extraction multilingue de:', textData);
    
    if (!textData) {
      console.log('❌ Pas de données texte');
      return '';
    }
    
    // Si c'est déjà une string simple
    if (typeof textData === 'string') {
      console.log('✅ String simple:', textData);
      return textData;
    }
    
    // Si c'est un objet multilingue du CMS
    if (typeof textData === 'object') {
      console.log('🌍 Objet multilingue détecté:', Object.keys(textData));
      
      // Essayer la langue courante d'abord
      if (textData[this.currentLanguage]) {
        console.log(`✅ Trouvé pour langue ${this.currentLanguage}:`, textData[this.currentLanguage]);
        return textData[this.currentLanguage];
      }
      
      // Essayer les variations de langues (fr-FR, fr-CH, etc.)
      const currentLangPrefix = this.currentLanguage.split('-')[0];
      for (const key of Object.keys(textData)) {
        if (key.startsWith(currentLangPrefix)) {
          console.log(`✅ Trouvé pour variation ${key}:`, textData[key]);
          return textData[key];
        }
      }
      
      // Fallback sur la première langue disponible
      const firstKey = Object.keys(textData)[0];
      if (firstKey && textData[firstKey]) {
        console.log(`📝 Fallback sur ${firstKey}:`, textData[firstKey]);
        return textData[firstKey];
      }
    }
    
    console.log('❌ Impossible d\'extraire le texte');
    return '';
  }

  // ==========================================
  // MÉTHODES POUR LE LAYOUT EXACT du slideshow
  // ==========================================

getContentProportion(contentType: 'first' | 'second'): string {
  if (!this.news || !this.newsData) return '50%';

  
  const box1Size = this.newsData.box1.size;
  const box2Size = 100 - box1Size;

  // Déterminer quelle box correspond à firstContent et secondContent
  if (this.newsData.box1.content === 'file') {
    // box1=file, box2=text → file=box1, text=box2
    // firstContent=image(box1), secondContent=texte(box2)
    return contentType === 'first' ? `${box1Size}%` : `${box2Size}%`;
  } else {
    // box1=text, box2=file → text=box1, file=box2  
    // firstContent=image(box2), secondContent=texte(box1)
    return contentType === 'first' ? `${box2Size}%` : `${box1Size}%`;
  }
}

  getFlexGrow(contentType: 'first' | 'second'): string {
    return '0'; // Pas de flex-grow, seulement flex-basis
  }

  // ==========================================
  // MÉTHODES UTILITAIRES
  // ==========================================

  hasAttachment(): boolean {
    return !!(this.news?.fileSrc && this.news.fileExtension);
  }

  isImage(): boolean {
    if (!this.news?.fileExtension) return false;
    const imageExtensions = ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp'];
    return imageExtensions.includes(this.news.fileExtension.toLowerCase());
  }

  isPdf(): boolean {
    return this.news?.fileExtension?.toLowerCase() === 'pdf';
  }

  translate(key: string): string {
    // Simple traduction pour les clés connues
    const translations: { [key: string]: string } = {
      'newsInternal.noNews': 'Aucune news disponible',
      'newsInternal.internal': 'Interne News'
    };
    return translations[key] || key;
  }

  closePreview(): void {
    window.close();
  }
} 