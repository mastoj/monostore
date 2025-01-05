import { Suspense } from 'react'
import CartsContent from './carts-content'
import Layout from '../components/layout'

export default function Carts() {
  return (
    <Layout>
      <h1 className="text-2xl font-bold mb-4">Carts Management</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <CartsContent />
      </Suspense>
    </Layout>
  )
}

