import {v4 as uuid}  from 'uuid'
export interface IBasketItem {
  id: number;
  productName: string;
  price: number;
  quantity: number;
  brand:string;
  type:string;
  pictureUrl: string;
}

export interface IBasket {
  id: string;
  items: IBasketItem[];
}

export class Basket implements IBasket{
  id=uuid();
  items:IBasketItem[]=[];
}


export interface IBasketTotals{
  shipping:number;
  subtotal:number;
  total:number;
}
