import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IBrand } from '../shared/models/brand';
import { IPagination } from '../shared/models/pagination';
import { IType } from '../shared/models/productType';
import { delay, map } from 'rxjs/operators';
import { ShopParams } from '../shared/models/shopParams';
import { IProduct } from '../shared/models/product';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl='https://localhost:5001/api/';

  constructor(private http:HttpClient) { }

  //getProducts(brandId?:number,typeId?:number,sort?:string){
    getProducts(shopParams:ShopParams){
    let params=new HttpParams();
    if(shopParams.brandId!==0)
    {
      params=params.append('brandId' , shopParams.brandId.toString());
    }
    if(shopParams.typeId !==0)
    {
      params=params.append('typeId',shopParams.typeId.toString());
    }
    if(shopParams.sort)
    {
      params=params.append('sort',shopParams.sort);
    }
    if(shopParams.search)
    {
      params=params.append('search',shopParams.search);
    }
    params=params.append('pageIndex',shopParams.pageNumber.toString());
    params=params.append('pageIndex',shopParams.pageSize.toString());

    return this.http.get<IPagination>(this.baseUrl + 'products',{ observe :'response',params})
    .pipe(
      delay(1000),
      map(response=>{
        return response.body;
      })
    );
  }

  getBrands(){
    return this.http.get<IBrand[]>(this.baseUrl + 'products/brands');
  }

  getTypes(){
    return this.http.get<IType[]>(this.baseUrl + 'products/types');
  }

  getProduct(id:number)
  {
    debugger;
    return this.http.get<IProduct>(this.baseUrl + 'products/' + id);
  }
}
