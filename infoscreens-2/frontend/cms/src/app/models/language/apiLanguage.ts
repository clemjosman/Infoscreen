import IStringDictionary from "../../interfaces/IStringDictionary";
import { apiLanguage_Light } from "@models/index";

export interface apiLanguage extends apiLanguage_Light {
  displayName: IStringDictionary<string>;
  cultureCode: string;
}
