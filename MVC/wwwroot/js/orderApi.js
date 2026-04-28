export const allHats = new Map();
export const allMaterials = new Map();

export const loadData = async () => {
    const [hatsRes, matsRes] = await Promise.all([
        fetch("/Order/GetAllHats"),
        fetch("/Order/GetAllMaterials")
    ]);

    const hats = await hatsRes.json();
    const mats = await matsRes.json();

    for (const d of hats) {
        const { id, ...rest } = d;
        allHats.set(id, rest);
    }

    for (const m of mats) {
        allMaterials.set(m.id, { name: m.name, unit: m.unit });
    }
};
