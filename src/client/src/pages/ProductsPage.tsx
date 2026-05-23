import { useEffect, useState } from 'react';

interface Product {
  id: number;
  name: string;
  price: number;
}

export default function ProductsPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch('/api/v1/products')
      .then((res) => {
        if (!res.ok) throw new Error(`Request failed: ${res.status}`);
        return res.json() as Promise<Product[]>;
      })
      .then((data) => setProducts(data))
      .catch((err: unknown) => setError(err instanceof Error ? err.message : 'Unknown error'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return <p style={{ padding: '1rem' }}>Loading products…</p>;
  }

  if (error !== null) {
    return (
      <p role="alert" style={{ padding: '1rem', color: '#c00' }}>
        Error: {error}
      </p>
    );
  }

  return (
    <main style={{ padding: '1rem' }}>
      <h1>Products</h1>
      <table style={{ borderCollapse: 'collapse', width: '100%' }}>
        <thead>
          <tr>
            {(['Id', 'Name', 'Price'] as const).map((col) => (
              <th
                key={col}
                style={{
                  textAlign: 'left',
                  padding: '0.5rem 1rem',
                  borderBottom: '2px solid #ccc',
                }}
              >
                {col}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {products.map((p) => (
            <tr key={p.id} style={{ borderBottom: '1px solid #eee' }}>
              <td style={{ padding: '0.5rem 1rem' }}>{p.id}</td>
              <td style={{ padding: '0.5rem 1rem' }}>{p.name}</td>
              <td style={{ padding: '0.5rem 1rem' }}>{p.price.toFixed(2)}</td>
            </tr>
          ))}
          {products.length === 0 && (
            <tr>
              <td colSpan={3} style={{ padding: '0.5rem 1rem', color: '#888' }}>
                No products found.
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </main>
  );
}
