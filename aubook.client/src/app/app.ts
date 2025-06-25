import { HttpClient, provideHttpClient } from '@angular/common/http';
import { Component, ElementRef, HostBinding, HostListener, inject, viewChild } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ButtonModule } from 'primeng/button';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet, ButtonModule],
    providers: [],
    templateUrl: './app.html',
    styleUrl: './app.scss',
})
export class App {
    audioPlayer = viewChild<ElementRef<HTMLAudioElement>>("audioPlayer");
    protected title = 'aubook.client';

    http = inject(HttpClient);

    ngOnInit() {
        this.http.get("/api/test/test", {
            // headers: {
            //     "Accecpt": "text/plain"
            // },
            responseType: "text"
        }).subscribe(console.log);
    }

    ngAfterViewInit() {
        document.addEventListener("keyup", (ev) => {
            if (ev.code == "Space") {
                const audioPlayer = this.audioPlayer()?.nativeElement;
                if (!audioPlayer) return;

                if (audioPlayer.paused) {
                    audioPlayer.play();
                } else {
                    audioPlayer.pause();
                }
            }
        })
        const audioPlayer = this.audioPlayer()?.nativeElement;
        if (!audioPlayer) return;

        this.http.get("/api/audio/stream", {
            "responseType": "blob"
        }).subscribe((res) => {
            const audioUrl = URL.createObjectURL(res);
            audioPlayer.src = audioUrl;
            audioPlayer.load()
        })

        // audioPlayer.src = "/api/audio/stream";
        // audioPlayer.load()
    }
}
