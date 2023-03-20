export type FilterRule<T = string> =
  | ValueFilterRule<T>
  | BoolFilterRule<T>
  | CollectionFilterRule<T>
  | ChildCollectionFilterRule<T>;

export interface ValueFilterRule<T = string, TValue = any> {
  fieldId: T;
  operator: '=' | '!=' | '>' | '>=' | '<' | '<=' | 'contains' | '!contains' | 'any' | 'all';
  value: TValue;
}

export interface BoolFilterRule<T = string> {
  operator: 'and' | 'or';
  children: FilterRule<T>[];
}

export interface CollectionFilterRule<T = string, TValue = any> {
  fieldId: T;
  operator: 'in' | '!in';
  values: TValue[];
}

export interface ChildCollectionFilterRule<T = string> {
  fieldId: T;
  operator: 'any' | 'all';
  rule: FilterRule;
}

export type SortDir = 'asc' | 'desc';

export interface SortRule<T = string> {
  fieldId: T;
  dir: SortDir;
}

export interface PagingRule {
  skip: number;
  take: number;
}
