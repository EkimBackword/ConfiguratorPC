import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { Compare } from './compare.model';

@Injectable()
export class CompareService {

	constructor(private http: Http) { }

	getList(): Observable<Compare[]> {
		return this.http.get('/api/list').map(res => res.json() as Compare[]);
	}
}