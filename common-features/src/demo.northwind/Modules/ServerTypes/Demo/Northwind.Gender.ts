import { registerEnum } from "@serenity-is/corelib";

export enum Gender {
    Male = 1,
    Female = 2
}
registerEnum(Gender, 'Serenity.Demo.Northwind.Gender');