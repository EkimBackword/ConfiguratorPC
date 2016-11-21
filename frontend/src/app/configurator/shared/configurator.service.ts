import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { Configurator } from './configurator.model';

@Injectable()
export class ConfiguratorService {

	constructor(private http: Http) { }

	getList(): Observable<Configurator[]> {
		return this.http.get('/api/list').map(res => res.json() as Configurator[]);
	}
}