import { NewsLayout } from "../enums/newsLayout";

export interface ActemiumNews {
  title: string;
  content: string;
  publicationDate: string;
  expirationDate: string;
  imgSrc: string;
  fileExtension: string;
}

export interface News {
  title: string;
  content: string;
  publicationDate: string;
  expirationDate: string;
  fileSrc: string;
  fileExtension: string;
  layout: NewsLayout;
  box1: NewsLayoutBox;
  box2: NewsLayoutBox;
}

export interface NewsLayoutBox {
  size: number;
  content: NewsLayoutContentType;
}

export enum NewsLayoutContentType {
  file = "file",
  text = "text"
}
