export interface Idea {
  id: number;
  title: string;
  text: string;
  status: string;
  originalLanguageCode: string;
  myIdea: boolean;
  averageRating: number;
  ratingCount: number;
  created: string;
  category: Idea_Category;
  currentStatus: Idea_Status;
  businessUnit: Idea_BusinessUnit;
}

export interface Idea_Category {
  id: number;
  name: string;
  color: string;
}

export interface Idea_Status {
  value: number;
  name: string;
  textCode: string;
  color: string;
}

export interface Idea_BusinessUnit {
  id: number;
  name: string;
  number: string;
  sourceId: string;
  sourceSystemTextCode: string;
  categories: Idea_Category[];
}
