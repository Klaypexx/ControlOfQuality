import readline from "readline";
import { Triangle } from "./triangle";
import { UNKNOWN_ERROR } from "./triangleTypes";

const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout,
  prompt: "",
});

rl.on("line", handleInput);

function handleInput(input: string): void {
  try {
    const sides = parseInput(input);
    validateSides(sides);
    const triangle = createTriangle(sides);
    triangle.calculateAndWriteType();
  } catch (error: any) {
    console.error(error.message);
  } finally {
    rl.close();
  }
}

function parseInput(input: string): number[] {
  return input.split(" ").map((side) => {
    const number = Number(side);
    if (isNaN(number)) {
      throw new Error(UNKNOWN_ERROR);
    }
    return number;
  });
}

function validateSides(sides: number[]): void {
  if (sides.length !== 3) {
    throw new Error(UNKNOWN_ERROR);
  }
}

function createTriangle(sides: number[]): Triangle {
  const [a, b, c] = sides;
  return new Triangle(a, b, c);
}
