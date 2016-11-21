import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { Auth } from './auth.model';

@Injectable()
export class AuthService {

	constructor(private http: Http) { }

	getList(): Observable<Auth[]> {
		return this.http.get('/api/list').map(res => res.json() as Auth[]);
	}
}