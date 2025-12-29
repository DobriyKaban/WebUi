export type FeatureType = 'toggle' | 'slider' | 'button' | 'dropdown' | 'color' | 'input' | 'dynamic_list';

export interface Feature {
  id: string;
  label: string;
  type: FeatureType;
  defaultValue?: any;
  min?: number;
  max?: number;
  options?: string[];
  keybind?: boolean; 
}

export interface Group {
  id: string;
  label: string;
  column: 0 | 1; // 0 for Left, 1 for Right
  features: Feature[];
}

export interface SubTab {
  id: string;
  label: string;
  icon: string;
  groups: Group[]; 
  customView?: 'players' | 'logs';
}

export interface MainTab {
  id: string;
  label: string;
  icon: string;
  subTabs: SubTab[];
}