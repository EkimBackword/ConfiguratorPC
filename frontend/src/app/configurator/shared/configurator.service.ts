import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

@Injectable()
export class ConfiguratorService {

	constructor(private http: Http) { }

	getNames(type): Observable<any> {
		return this.http.get('http://localhost:27835/api/' + type).map(res => res.json());
	}

	getList(type, data): Observable<any> {
		let headers = new Headers({ 'Content-Type': 'application/json' });
    	let options = new RequestOptions({ headers: headers });

		return this.http.post('http://localhost:27835/api/' + type, JSON.stringify(data), options)
			.map(res => res.json());
	}
}