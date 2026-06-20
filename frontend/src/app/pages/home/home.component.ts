import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { Fruit } from '../../models/fruit';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  fruits: Fruit[] = [];
  heroImageUrl = '/assets/Image/Shop Image.jpg';

  constructor(private api: ApiService, public auth: AuthService) {}

  ngOnInit(): void {
    this.api.getFruits().subscribe((response: any) => {
      this.fruits = response.data || [];
    });
  }
}
