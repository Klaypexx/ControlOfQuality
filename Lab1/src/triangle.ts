import {
  EQUILATERAL_TYPE,
  ISOSCELES_TYPE,
  NOT_A_TRIANGLE_TYPE,
  ORDINARY_TYPE,
  UNKNOWN_ERROR,
  UNSIGNED_TYPE,
} from "./triangleTypes";

export class Triangle {
  private a: number;
  private b: number;
  private c: number;
  private type: string;

  constructor(a: number, b: number, c: number) {
    this.a = a;
    this.b = b;
    this.c = c;
    this.type = UNSIGNED_TYPE;
  }

  async calculateAndWriteType() {
    try {
      this.typeCalculation();
      console.log(this.type);
    } catch (e) {
      console.log(UNKNOWN_ERROR);
    }
  }

  private typeCalculation() {
    if (this.hasZeroSides()) {
      this.type = NOT_A_TRIANGLE_TYPE;
    } else if (this.isEquilateral()) {
      this.type = EQUILATERAL_TYPE;
    } else if (this.isIsosceles()) {
      this.type = ISOSCELES_TYPE;
    } else {
      this.type = ORDINARY_TYPE;
    }
  }

  private hasZeroSides(): boolean {
    return this.a <= 0 || this.b <= 0 || this.c <= 0;
  }

  private isEquilateral(): boolean {
    return this.a === this.b && this.b === this.c;
  }

  private isIsosceles(): boolean {
    return this.a === this.b || this.a === this.c || this.b === this.c;
  }

  getType(): string {
    return this.type;
  }
}
