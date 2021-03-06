import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { VisualizationComponent } from "./visualization.component";
import { VisualizationRoutingModule } from "./visualization.routing.module";
import { SideNavOuterToolbarModule, SideNavInnerToolbarModule } from "../layouts";
import { FooterModule } from "../shared/components/footer/footer.component";
import { RouterModule } from "@angular/router";
import { TranslationModule } from "angular-l10n";

@NgModule({
  declarations: [VisualizationComponent],
  imports: [
    CommonModule,
    VisualizationRoutingModule,
    SideNavOuterToolbarModule,
    SideNavInnerToolbarModule,
    FooterModule,
    RouterModule,
    TranslationModule
  ],
  exports: [
    VisualizationComponent
  ]
})
export class VisualizationModule { }
