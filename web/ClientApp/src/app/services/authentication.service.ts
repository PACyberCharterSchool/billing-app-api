import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';
import { User } from '../models/user.model';

import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable()
export class AuthenticationService {
  private apiLoginUrl = '';
  constructor(private http: HttpClient) {
    this.apiLoginUrl = environment.apiBaseUrl + '/api/auth/login';
  }

  public isAuthenticated(): boolean {
    const token = localStorage.getItem('jwt-token');
    const jwtHelper = new JwtHelperService();

    console.log('AuthenticationService.isAuthenticated():  token is ', token);

    // returns true or false based upon whether the JWT is empty or expired...
    return ((token !== null) || !jwtHelper.isTokenExpired(token));
  }

  public authenticate(email: string, password: string): Observable<any> {
    const headers = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };

    const credentials = { username: email, password: password };
    console.log('AuthenticationService.authenticate():  apiLoginUrl is ', this.apiLoginUrl);
    return this.http.post(this.apiLoginUrl, JSON.stringify(credentials), headers);
  }
}