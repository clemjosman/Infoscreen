import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { TranslationService } from '@vesact/web-ui-template';

interface PreviewDeviceData {
  deviceType: 'tv';
  width: number;
  height: number;
  name: string;
  description: string;
}

@Component({
  selector: 'app-device-preview-dialog',
  template: `
    <div class="modern-dialog">
      <div class="dialog-header">
        <div class="header-content">
          <div class="header-icon">
            <mat-icon>tv</mat-icon>
          </div>
          <div class="header-text">
            <h2>Prévisualisation TV</h2>
            <p>Choisissez la résolution d'écran pour voir le rendu exact</p>
          </div>
        </div>
        <button mat-icon-button (click)="onCancel()" class="close-button">
          <mat-icon>close</mat-icon>
        </button>
      </div>
      
      <mat-dialog-content class="dialog-body">
        
        <!-- Résolutions d'écran -->
        <div class="section">
          <h3 class="section-title">
            <mat-icon>tv</mat-icon>
            Résolutions d'écran
          </h3>
          <div class="preset-list">
            <div 
              *ngFor="let preset of allPresets" 
              class="preset-row"
              [class.selected]="selectedPreset === preset"
              (click)="selectPreset(preset)">
              <mat-icon class="preset-row-icon">tv</mat-icon>
              <div class="preset-row-info">
                <span class="preset-row-name">{{ preset.name }}</span>
                <span class="preset-row-resolution">{{ preset.width }}×{{ preset.height }}</span>
              </div>
              <div class="preset-row-description">{{ preset.description }}</div>
              <mat-icon 
                class="preset-row-check" 
                *ngIf="selectedPreset === preset">check_circle</mat-icon>
            </div>
          </div>
        </div>

        <!-- Dimensions personnalisées -->
        <div class="section">
          <h3 class="section-title">
            <mat-icon>tune</mat-icon>
            Dimensions personnalisées
          </h3>
          <div class="custom-card" 
               [class.selected]="isCustomPreset()"
               (click)="selectCustomPreset()">
            <div class="custom-header">
              <mat-icon>settings</mat-icon>
              <span>Saisir des dimensions spécifiques</span>
              <mat-icon 
                class="custom-check" 
                *ngIf="isCustomPreset()">check_circle</mat-icon>
            </div>
            
            <div class="custom-inputs" *ngIf="isCustomPreset()">
              <div class="input-group">
                <mat-form-field appearance="outline">
                  <mat-label>Largeur</mat-label>
                  <input matInput 
                         type="number" 
                         [(ngModel)]="customWidth"
                         min="800" 
                         max="7680"
                         (ngModelChange)="onCustomDimensionsChange()">
                  <span matSuffix>px</span>
                </mat-form-field>
                
                <span class="separator">×</span>
                
                <mat-form-field appearance="outline">
                  <mat-label>Hauteur</mat-label>
                  <input matInput 
                         type="number" 
                         [(ngModel)]="customHeight"
                         min="600" 
                         max="4320"
                         (ngModelChange)="onCustomDimensionsChange()">
                  <span matSuffix>px</span>
                </mat-form-field>
              </div>
            </div>
          </div>
        </div>
        
        <!-- Aperçu de la sélection -->
        <div class="preview-summary" *ngIf="selectedPreset">
          <div class="summary-content">
            <mat-icon class="summary-icon">tv</mat-icon>
            <div class="summary-text">
              <div class="summary-title">Rendu sur {{ getSelectedName() }}</div>
              <div class="summary-details">{{ getFinalDimensions().width }}×{{ getFinalDimensions().height }}px • {{ getAspectRatio() }}</div>
            </div>
          </div>
        </div>
        
      </mat-dialog-content>
      
      <mat-dialog-actions class="dialog-actions">
        <button mat-button (click)="onCancel()" class="cancel-button">
          Annuler
        </button>
        <button 
          mat-raised-button 
          color="primary" 
          (click)="onLaunchPreview()"
          [disabled]="!isValidDimensions()"
          class="launch-button">
          <mat-icon>launch</mat-icon>
          Lancer la prévisualisation
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styleUrls: ['./device-preview-dialog.component.scss']
})
export class DevicePreviewDialogComponent {
  
  selectedPreset: PreviewDeviceData | null = null;
  customWidth: number = 1920;
  customHeight: number = 1080;
  
  // Toutes les résolutions en une seule liste
  allPresets: PreviewDeviceData[] = [
    { 
      deviceType: 'tv', 
      width: 1920, 
      height: 1080, 
      name: 'Full HD', 
      description: 'Standard HD le plus courant'
    },
    { 
      deviceType: 'tv', 
      width: 3840, 
      height: 2160, 
      name: '4K UHD', 
      description: 'Ultra haute définition'
    },
    { 
      deviceType: 'tv', 
      width: 2560, 
      height: 1440, 
      name: 'QHD', 
      description: 'Qualité haute définition'
    },
    { 
      deviceType: 'tv', 
      width: 1366, 
      height: 768, 
      name: 'HD Ready', 
      description: 'Écrans anciens'
    },
    { 
      deviceType: 'tv', 
      width: 1280, 
      height: 720, 
      name: 'HD 720p', 
      description: 'Écrans basiques'
    },
    { 
      deviceType: 'tv', 
      width: 1600, 
      height: 900, 
      name: 'HD+', 
      description: 'Format intermédiaire'
    }
  ];
  
  customPreset: PreviewDeviceData = { 
    deviceType: 'tv', 
    width: 0, 
    height: 0, 
    name: 'Personnalisé', 
    description: 'Dimensions spécifiques'
  };
  
  constructor(
    public dialogRef: MatDialogRef<DevicePreviewDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { newsData: any },
    private router: Router,
    private translationService: TranslationService
  ) {
    // Sélectionner Full HD par défaut
    this.selectedPreset = this.allPresets[0];
  }
  
  selectPreset(preset: PreviewDeviceData): void {
    this.selectedPreset = preset;
  }
  
  selectCustomPreset(): void {
    this.customPreset.width = this.customWidth;
    this.customPreset.height = this.customHeight;
    this.selectedPreset = this.customPreset;
  }
  
  onCustomDimensionsChange(): void {
    if (this.isCustomPreset()) {
      this.customPreset.width = this.customWidth;
      this.customPreset.height = this.customHeight;
    }
  }
  
  isCustomPreset(): boolean {
    return this.selectedPreset === this.customPreset;
  }
  
  getFinalDimensions(): { width: number; height: number } {
    if (!this.selectedPreset) return { width: 1920, height: 1080 };
    
    if (this.isCustomPreset()) {
      return { width: this.customWidth, height: this.customHeight };
    }
    return { width: this.selectedPreset.width, height: this.selectedPreset.height };
  }
  
  getSelectedName(): string {
    if (!this.selectedPreset) return 'Aucune sélection';
    return this.selectedPreset.name;
  }
  
  getAspectRatio(): string {
    const { width, height } = this.getFinalDimensions();
    const gcd = (a: number, b: number): number => b === 0 ? a : gcd(b, a % b);
    const divisor = gcd(width, height);
    const ratioW = width / divisor;
    const ratioH = height / divisor;
    
    if (ratioW === 16 && ratioH === 9) return 'Format 16:9';
    if (ratioW === 4 && ratioH === 3) return 'Format 4:3';
    if (ratioW === 21 && ratioH === 9) return 'Format 21:9';
    return `Format ${ratioW}:${ratioH}`;
  }
  
  isValidDimensions(): boolean {
    const dims = this.getFinalDimensions();
    return dims.width >= 800 && dims.height >= 600 && 
           dims.width <= 7680 && dims.height <= 4320;
  }
  
  onCancel(): void {
    this.dialogRef.close({ launched: false });
  }
  
  onLaunchPreview(): void {
    if (!this.isValidDimensions() || !this.selectedPreset) return;
    
    try {
      const deviceData = {
        deviceType: 'tv' as const,
        width: this.getFinalDimensions().width,
        height: this.getFinalDimensions().height
      };
      
      const currentLanguage = this.translationService.currentLanguage.value?.iso2 || 'fr';
      
      const queryParams = {
        news: encodeURIComponent(JSON.stringify(this.data.newsData)),
        device: encodeURIComponent(JSON.stringify(deviceData)),
        language: currentLanguage
      };
      
      const url = this.router.serializeUrl(
        this.router.createUrlTree(['/news-preview-exact'], { queryParams })
      );
      
      window.open(url, '_blank', `width=${deviceData.width},height=${deviceData.height},scrollbars=no,resizable=yes`);
      
      this.dialogRef.close({ launched: true });
      
    } catch (error) {
      console.error('Erreur lors de l\'ouverture de la prévisualisation:', error);
    }
  }
}