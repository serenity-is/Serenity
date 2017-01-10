declare namespace Serenity {

    namespace FilteringTypeRegistry {
        function get(key: string): Function;
        function initialize(): void;
        function reset(): void;
    }

}