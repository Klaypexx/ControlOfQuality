import * as fs from "fs";
import { Triangle } from "../triangle";

const INPUT_FILE = "input.txt";
const OUTPUT_FILE = "output.txt";
const SUCCESS = "success";
const ERROR = "error";

describe("Triangle Type Calculation", () => {
  it("should correctly determine the type of triangles from input file", async () => {
    const inputLines = readInputFile(INPUT_FILE);
    const outputResults: string[] = [];

    for (const line of inputLines) {
      const { a, b, c, expectedType } = parseInputLine(line);
      const result = checkTriangleType(a, b, c, expectedType);
      outputResults.push(result);
    }

    writeOutputFile(OUTPUT_FILE, outputResults);
  });
});

function readInputFile(filePath: string): string[] {
  return fs.readFileSync(filePath, "utf-8").trim().split("\n");
}

function writeOutputFile(filePath: string, results: string[]): void {
  fs.writeFileSync(filePath, results.join("\n"), "utf-8");
}

function parseInputLine(line: string): {
  a: number;
  b: number;
  c: number;
  expectedType: string;
} {
  const parts = line.split(" ");
  const a = Number(parts[0]);
  const b = Number(parts[1]);
  const c = Number(parts[2]);
  const expectedType = parts.slice(3).join(" ");
  return { a, b, c, expectedType };
}

function checkTriangleType(
  a: number,
  b: number,
  c: number,
  expectedType: string
): string {
  const triangle = new Triangle(a, b, c);
  triangle.calculateAndWriteType();
  const actualType = triangle.getType().trim().toLocaleLowerCase();
  return actualType === expectedType.trim().toLocaleLowerCase()
    ? SUCCESS
    : ERROR;
}
