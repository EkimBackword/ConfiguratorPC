import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { Examples } from './examples.model';

@Injectable()
export class ExamplesService {

	constructor(private http: Http) { }

	getList(): Observable<Examples[]> {
		return this.http.get('/api/list').map(res => res.json() as Examples[]);
	}
}