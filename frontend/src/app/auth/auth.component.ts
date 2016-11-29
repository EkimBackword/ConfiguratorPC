import { Component, OnInit } from '@angular/core';

import { Auth } from './shared/auth.model';
import { AuthService } from './shared/auth.service';

@Component({
	selector: 'auth',
	templateUrl: 'auth.component.html',
	styleUrls: ['./auth.component.less'],
	providers: [AuthService]
})

export class AuthComponent implements OnInit {
	auth: Auth[] = [];
	user: any = {
		login: null,
		pass: null
	}

	constructor(private authService: AuthService) {}

	ngOnInit() {

	}
}