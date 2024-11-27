import { SettingStorage } from "@serenity-is/corelib";
import { UserPreferenceService } from "../ServerTypes/Extensions/UserPreferenceService";
import { UserPreferenceRetrieveResponse } from "../ServerTypes/Extensions";

export class UserPreferenceStorage implements SettingStorage {
    async getItem(key: string): Promise<string> {
        const response = await Promise.resolve(UserPreferenceService.Retrieve({
            PreferenceType: "UserPreferenceStorage",
            Name: key
        }) as PromiseLike<UserPreferenceRetrieveResponse>);
        return response.Value;
    }

    async setItem(key: string, data: string): Promise<void> {
        return Promise.resolve(UserPreferenceService.Update({
            PreferenceType: "UserPreferenceStorage",
            Name: key,
            Value: data
        }) as any);
    }
}